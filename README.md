# BandLab Test Assignment
Test assignment for the Backend Engineer position in BandLab


## Task description

### What to build:
- A system that allows you to upload images and comment on them
- No frontend/UI is required

### User stories (where the user is an API consumer):
- As a user, I should be able to create posts with images (1 post - 1 image)
- As a user, I should be able to set a text caption when I create a post
- As a user, I should be able to comment on a post
- As a user, I should be able to delete a comment (created by me) from a post
- As a user, I should be able to get the list of all posts along with the last 2 comments on each post

### Functional requirements:
- RESTful Web API (JSON)
- Maximum image size - 100MB
- Allowed image formats: .png, .jpg, .bmp.
- Save uploaded images in the original format
- Convert uploaded images to .jpg format and resize to 600x600
- Serve images only in .jpg format
- Posts should be sorted by the number of comments (desc)
- Retrieve posts via a cursor-based pagination

### Non-functional requirements:
- Maximum response time for any API call except uploading image files - 50 ms
- Minimum throughput handled by the system - 100 RPS
- Users have a slow and unstable internet connection

### Usage forecast:
- 1k uploaded images per 1h
- 100k new comments per 1h

### Domain:
#### Post:
- Id
- Caption (string)
- Image
- Creator
- CreatedAt (datetime)
- Comments (list)
#### Comment:
- Id
- Content (string)
- Creator
- CreatedAt (datetime)

### Preferable tools:
- .NET / Node.js / Go
- Azure Functions / AWS Lambda / GCP Cloud Run / k8s / Docker

### Expected result:
- Source code: make a public GitHub repo or share it with our user (i-am-from-bandlab)
- Whatever you think is necessary to deliver a prototype/MVP of this project
- Details about what should be done in order to ship it on production

### Notes:
- You are recommended to spend no more than 6 hours on this challenge
- You don’t have to complete all the stories - the goal is to show your strengths
- Focus on good design and clean implementation
- Develop your system applying the best software development practices
- The non-functional requirements and the usage forecast should be only considered
as design guidelines - there is no need to prove it
- If you have any questions feel free to ask


## Implementation

It is recommended to spend not more than 6 hours for this task but obviously it’s impossible to implement anything working within this time. So I will focus on making some simplest MVP covering most of the requirements as quickly as possible. 

I am avoiding using technologies and approaches that I am unfamiliar with as research and setting up will take too much time.

### Initial setup

User stories clearly define the set of endpoints that we need. Whatever implementation would be we need those endpoints one way or another. So first of all I created the basic ASP.NET Core Web API with mocked endpoints which can be called and return some empty response.

We need scaling and platform independence potentially, so I will add Docker support from the start.

I will not be implementing authorization as it is a complex task and we definitely don’t have time for it. For now I will be just sending parameter `author` in request and will be treating it as it was received from claims after authorization.

### Posts endpoints

We start with basic Web API for posts with the following endpoints:

```jsx
// For getting paginated posts feed
GET /api/posts

// For creating the new post
POST /api/posts
```

Posts in feed should be retrieved using cursor pagination and sorted by comments count. So for query parameters we surely have `limit` which can be made optional and `cursorCommentsCount` for all the pages except the first one. But we will need to add another property for cursor because multiple posts can have the same comments count. I will go with sorting by Id in the ascending order as it matches the physical records order in the table. So the third parameter will be the `cursorId`.

Post without the image does not make sense and there is no mention of separate image upload process so to combine it all in the same POST request with other metadata I will go with `Content-Type: multipart/form-data` for this endpoint as it is convenient to use in tools like Postman etc.

### Comments endpoints

Conceptually comments have many-to-one relation to posts. It doesn’t necessarily mean they should have physical relation in DB but from endpoints routing perspective comments should be the URL segment going after posts.

For start I will just create the following comments endpoints in the same API for posts:

```jsx
// For creating the comment under post
POST /api/posts/{postId}/comments

//For deleting the comment
DELETE /api/posts/{postId}/comments/{id}
```

### Docker Compose

The next step is to setup the orchestration. I saw Kubernetes as the recommendation in description but I am not sufficiently familiar with it so I will go with Docker Compose. 

I will create only `docker-compose.yml` for simplicity and all the env variable will come from `.env` file. But in real life there could be more complex structure with separate docker compose files for each purpose.

### Asynchronous operations

Response should be almost instantaneous, so all potentially long operations (image conversion, DB calls etc.) should be asynchronous. 

It makes sense to go with event-based approach and leverage some cloud resources. For example we could save original image in some blob storage and fire the event for creating the post, it would use the source image URL to asynchronously retrieve and convert it, get the new URL and save in the post. It would also allow us to leverage CDN nodes for image caching on transport level if a lot of people requests the feed. 

But unfortunately I don’t have a lot of hands on experience with cloud services and event-based systems setup, things like Kafka etc. and I don’t have such luxury as time for investigation so I would go with simple `Rabbit` messaging for speed and ease of implementation.

API calls that suppose to make modifications will just fire some event by producing a new message in Rabbit. Corresponding even consumers will retrieve the message and do all the long operations on the background.

We can add some statuses for events processing and we also can add some endpoints for checking it but for simplicity statuses are moved out of the scope. 

We would probably want to scale consumers and API instances independently but for now we put consumers in the same project just to test the setup. Rabbit itself will not have sophisticated implementation for now, no dead letter queue handling etc. - just the simplest approach.

### Consumers

If posts should be sorted by number of comments it makes sense to pre-calculate this value for the post after each new comment and store it.

There will be 3 main type of consumers:

- **For handling post creation:** save source image → get URL → convert image → get new URL → save post with new URL for image in DB
- **For handling comment creation:** save comment in DB → update comments count
- **For handling comment deletion:** remove comment from DB → update comments count

For start we just put `Task.Delay(500)` in consumers to imitate the activity.

### Storage

For posts we can use normal SQL storage as the amount of operations not incredibly high. For comments we might want to use something more efficient. I would either made a performance testing and comparison when implementing it in SQL and in something different - MongoDB, DynamoDB etc. or spent some time on research and got the proven suggestions.

It’s not very clear at this point how we are going to include 2 comments for each post in feed if comments are not in the same DB so for start and testing we can keep this idea aside and begin with normal SQL storage.

I will just use `MS SQL Server` as my data provider just because I have more experience with it than with others, but in general we can use anything. 

I will also use `Entity Framework` as ORM because of its convenience of use and ability to use `EF Migrations` to keep changes history. We will be applying migrations on API startup for simplicity and use single `sa` user for all databases.

### Post entity

Lets decide what type to use for ID. 

`int` is the most convenient type as it will match the physical order of records. Considering creation time is also matches the records order it might be helpful in some requests.

But is `int` sufficient?

Forecast for images 1k per 1h. 24k per day. 9kk per year.

Posts are 1-1 to images. Max value of `int` is 2kkk+ so so we shouldn’t run out of values in next 200 years which is, I guess, acceptable.

### Comment entity

Amount of comments is big so it makes sense to take `long` as Id type. It could be `Guid` but it’s not very convenient to sort.

### Creating the post

Now that we have the DB in place we can proceed with the implementation of consumers.

For start we implement Create Post consumer to just accept the message with post data and insert the record into DB. Storing the initial image should be initiated at that point already and we should have the source image URL - we cannot just send the image itself in the message.

Since we don’t have much time I will just mock the source image URL implying it’s somewhere in blob storage and has static URL.

Part with image conversion is also going to take decent amount of time for investigation so we will omit this part for now.

No error handling so far.

### Creating the comment

First, we insert the comment record into table. Then, we updating the comments count.

Our system will be under high load so we don’t want to use transactions here. In the same time we want to be as consistent as possible. So we detach first operation from second and will be committing them one by one. 

For counter update we will use `UPDATE … WHERE` approach as it will allow us to read the current value and increment it without locks but with the atomicity guaranteed.

Ideally we need to chain these operations and handle the case when comments are updated but counters are not. But proper error handling is the complex thing so we are not focusing on it now.

### Deleting the comment

Basically the approach is the same as for creation: we delete the comment first and then decrement the comments count in the post.

### Getting page of posts

The basic implementation of this endpoint will be to just get the specified amount of posts starting from some comments count and ID ordered by these properties. The last element will define the next `CursorCommentCount` and `CursorId`.

### Including comments in post

As we need to display 2 latest comments on each posts we need to use `JOIN` and that means we need to add index on `PostId`.

### Architecture of the system

Now we have basic working implementation. But we have functional and non-functional requirements which may imply different degree of scaling for the different parts of system.

Now it becomes more clear that it makes sense to create a separate `Gateway` service which will accept the request and initiate the processes responding very quickly. It should serve a lot of requests so it may have different configuration.

The message consumers probably should go in different services.

Remember that we have 50ms limitation on responses. We sorted out data modifying endpoints but we also have endpoint for getting feed. We want it to be quick too. And that’s the moment to introduce caching.

Cache would store the pages of posts feed and probably it makes sense to create a separate `FeedAPI`, but for now we keep it in `PostsAPI`. Caching will be managed on `Gateway`.

Comments now in SQL but in general it can be stored in different database. Their requests count is also different so separate `CommentsAPI` would make sense. For now we keep it in `PostsAPI`.

Image processing can be hosted in separate service too but we have not implemented it yet.

So to sum up what we have now:

**Services**

- Gateway (RESTful API for users)
- Posts (potentially called from Feed)
- Comments (not implemented, potentially called from Feed)
- Feed (not implemented, potentially called from Gateway, uses cache)
- Image Processor (not implemented, potentially called from Gateway)

**Infrastructure**

- MS SQL Server (+ other DBs to be considered)
- RabbitMQ (+ switching to full-fledged event-based system to be considered)
- Redis (+ more sophisticated cache to be considered)

### Caching

I will just use simple `Redis` for distributed cache. We can’t have service instance cache because of scaling and potential accessing/invalidating the cache from different services.

The corner stone of caching here is how to properly invalidate cache records on data changes. We do not want to invalidate all the cache - changes frequency is too high. It would be great to invalidate only specific cache partitions.

I am sure there are more convenient tools to solve this problem, some dimensional caches with index searches but I needed something super quick.

So my workaround is following:

- For each cached page request I store the cache key in the cache sets. One set for each comments count. So if page returns posts with comments counts 5, 5, 2, 1 any change that introduce post with comments count 1, 2, 3, 4, 5 will modify the page result returned on that request. So we add this page in sets or “buckets” with all numbers from 1 to 5. Not the best solution, more like hack, but it closes the main need.
- Another case is when the cached page contains less items that were requested. In this case basically any new post with comments count less than cursor comments count would modify the page - even if there are no comments at all - it will be added in the end of list.
- Each post creation will trigger cache invalidation for all the pages with bucket “0”.
- Each comment creation will trigger cache invalidation for all the keys in buckets “N” and “N+1” where N - original comments count in the post
- Each comment deletion will trigger cache invalidation for all the keys in buckets “N” and “N-1” where N - original comments count in post

### Running the solution

You need to have Docker installed on your machine.

First, let’s build the solution. From repository root folder run:

```bash
docker compose build
```

Then run it:

```bash
docker compose up -d
```

Containers data is persisted in `/volumes`.

Environment variables are taken from `.env`.

To access swagger on [localhost](http://localhost) you need to expose API ports. For now only Gateway is exposed.

### Conclusion

Now we have a working MVP which aligns with most of the requirements. Not covering image processing part. 

Some tools can be changed, some additional services can be added, but in general the solution has sufficient modularity already to be easily split and scaled in the future. 

### TODO

- Image processing
- Certificate for https
- More robust Rabbit implementation
- Create separate user for each DB
- Text fields length limitation
- Auto-migrations → migration script/separate migration worker
- Error handling
- Retries policies
- Separate solutions for services
- Code documentation
- Unit tests
- Logs, metrics, and traces - I would go with OpenTelemetry
- Performance testing
- Load balancing and reverse proxying
- Authentication
