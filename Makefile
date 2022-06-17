.RECIPEPREFIX=>
VERSION=1.0.0

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
> docker run -v $(shell pwd)/transfer:/transfer backup-utility:$(VERSION)
> chown -R pyrix25633:pyrix25633 ./transfer/docker/*

run-dotnet-debug:
> dotnet ./transfer/docker/debug/backup-utility.dll

run-dotnet-release:
> ./transfer/docker/release/linux-x64/backup-utility