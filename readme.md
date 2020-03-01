# EventBus

## rabbitmq

```
docker run -d --hostname my-rabbit --name rabbit \
    -p 4369:4369  -p 5671:5671 -p 5672:5672 -p 25672:25672 -p 15671:15671 \
    -e RABBITMQ_DEFAULT_USER=user \
    -e RABBITMQ_DEFAULT_PASS=password \
    rabbitmq:3-management

```