# Learning about scaling a Software System

This repository contains my experiments around scaling software systems.

Currently, I am working on building a BASE API that will serve as the basis of my future experiments, which will be about scaling up a software system.

Eventually, I will be benchmarking BASE against other implementations (using message queues, caching, ...) to learn about the impact of said implementations.

---

The context of the application is a simple social media application, with at least the following endpoints:

| Endpoint | Method | Explanation |
| --- | --- | --- |
| /posts | GET | Gets the feed (latest 20 posts) |
| /posts/search | GET / QUERY | Search the feed |
| /posts/{id} | GET | Opens a specific post |
| /posts | POST | Create a new post |
| /posts/like/{postid} | POST | Like a post |
| /posts/reply/{postid} | POST | reply to a post |
| /users/follow/{userid} | POST | follow a user |

I would want to try out the new QUERY method, but its implemented nowhere lol

Why is every company only focussing on AI

## BASE

> /social-media

This folder contains the ‘base’ version of the application. This is a Domain Driven Design implementation of the endpoints. The architecture choice is overkill, but it makes it easy to convert into other architectures. This will make the development time for my experiments (cache, message queues, …) as short as possible

| Status | Missing Endpoints |
| --- | --- |
| In progress | /posts/search | 
| | /users/follow/{userid} |

## KAFKA

> /social-kafka

This folder contains the ‘kafka’ version of the application.

| Status | Missing Endpoints |
| ------ | ----------------- |
| In Progress | /posts (method: POST) |
| | /posts/like/{postid} |
| | /posts/reply/{postid} |
| | /users/follow/{userid} |

## LOAD-TESTER

> /load-tester

This contains the K6 benchmark that test the applications' performances.

| Status |
| --- |
| DONE |

## DB SEEDER

This contains a simple program that seeds the database before testing. (To get valid tests, the starting databases should be the same)

| Status |
| --- |
| DONE |
