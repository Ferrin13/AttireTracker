VERSION_NUMBER=${1:-'0.1.0'}

docker build . --tag attire-tracker:$VERSION_NUMBER
