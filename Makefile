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

run-dotnet-debug:
> dotnet ./transfer/docker/debug/backup-utility.dll

run-dotnet-release:
> ./transfer/docker/release/linux-x64/backup-utility