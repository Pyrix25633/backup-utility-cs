#!/bin/bash
dotnet publish -c release -r linux-x64 -p:Version=$1 -p:PublishSigleFile=true --self-contained true -p:PublishReadyToRun=true
dotnet publish -c release -r win-x64 -p:Version=$1 -p:PublishSigleFile=true --self-contained true -p:PublishReadyToRun=true
rm -r -v /transfer/docker/release/*
cp -r -v /app/bin/release/net6.0/* /transfer/docker/release