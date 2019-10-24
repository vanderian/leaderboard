# leaderboard
example leaderboard service using Orleans actors with gRPC

use `docker-compose up` to run locally, the API will be at `localhost:5000`


use `make` for local deploy to kubernetes, a local cluster needs to be running.
You can use [microk8s](https://microk8s.io/) for example.
Once deployed use `kubectl` cli to get API url.  