#!/bin/bash
cd /transfer/local
chmod +x *.sh
source options.sh

if [ $RELEASE -eq 1 ]
then
    ./compile-release.sh $VERSION
else
    ./compile-debug.sh $VERSION
fi