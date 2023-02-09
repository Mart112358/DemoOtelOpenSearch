FROM fluent/fluentd:v1.15.3-debian-1.1

USER root

# RUN apt update && apt upgrade -y && apt install curl
RUN apt update && apt upgrade -y && apt install curl -y
# RUN gem install fluent-plugin-opensearch
RUN gem install fluent-plugin-opensearch
# RUN fluent-gem install fluent-plugin-rewrite-tag-filter fluent-plugin-multi-format-parser


