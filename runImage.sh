VERSION_NUMBER=${1:-'0.1.0'}

docker run -p 9091:8080 attire-tracker:$VERSION_NUMBER \
    -e INTERNAL_PORT=8080
