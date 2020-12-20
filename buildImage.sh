VERSION_NUMBER=${1:-'0.3.0'}

docker build . --tag attire-tracker:$VERSION_NUMBER
