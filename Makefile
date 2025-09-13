.RECIPEPREFIX=>
VERSION=1.6.4

init:
> mkdir transfer/docker
> mkdir transfer/docker/debug
> mkdir transfer/docker/release

default:
> clear
> make build-image
> make run-container
> make run-dotnet-debug

release:
> clear
> make build-image
> make run-container
> make run-dotnet-release

build-image:
> docker build -t backup-utility:$(VERSION) .

run-container:
> docker run --mount type=bind,source=$(shell pwd)/transfer,target=/transfer backup-utility:$(VERSION)
> sudo chown -R pyrix25633:pyrix25633 ./transfer/docker/*

run-dotnet-debug:
> dotnet ./transfer/docker/debug/backup-utility.dll -- -s test/source -d test/destination -r test/removed -t 100 -e extensions.txt -l -b

run-dotnet-release:
> ./transfer/docker/release/linux-x64/backup-utility --help