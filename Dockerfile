FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app
COPY *.csproj ./
COPY *.cs ./
COPY launch-compilation.sh ./
RUN chmod +x /app/launch-compilation.sh

CMD [ "/app/launch-compilation.sh" ]