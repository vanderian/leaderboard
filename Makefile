# your Docker registry (we suppose you have logged in and have push access)
# REGISTRY ?= docker.io/username
# the context of your local Kubernetes cluster
LOCAL_REGISTRY=localhost:32000
LOCAL_K8S_CLUSTER=microk8s
# the contect of your remote Kubernetes cluster
# REMOTE_K8S_CLUSTER=remote-cluster
# version/tag of the images that will be pushed to Docker Hub
#VERSION=0.0.1

API_PROJECT_NAME=pong-api
SILO_PROJECT_NAME=leaderboard-silo

# tag of the images that will be pushed for local development
TAG?=$(shell git rev-list HEAD --max-count=1 --abbrev-commit)
TAG_API=$(LOCAL_REGISTRY)/$(API_PROJECT_NAME):$(TAG)
TAG_SILO=$(LOCAL_REGISTRY)/$(SILO_PROJECT_NAME):$(TAG)

export ASPNETCORE_ENVIRONMENT=k8

foo: 
		@echo $(TAG_API)

buildlocal:
		docker build -f ./src/Silo/Dockerfile -t $(TAG_SILO) --build-arg ASPNETCORE_ENVIRONMENT .
		docker build -f ./src/Api/Dockerfile -t $(TAG_API) --build-arg ASPNETCORE_ENVIRONMENT .
#		docker system prune -f
		
pushlocal:
		docker push $(TAG_SILO)
		docker push $(TAG_API)

deploylocal: uselocalcontext
		sed -e 's=orleans-silo-image=$(TAG_SILO)=' -e 's=orleans-api-image=$(TAG_API)=' deploy.yaml | kubectl apply -f -

cleandeploylocal: uselocalcontext clean

uselocalcontext: 
		kubectl config use-context $(LOCAL_K8S_CLUSTER)

clean:
		kubectl delete deployment $(SILO_PROJECT_NAME) $(API_PROJECT_NAME)
		kubectl delete service $(SILO_PROJECT_NAME) $(API_PROJECT_NAME)	