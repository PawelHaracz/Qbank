
FROM eventstore/eventstore AS base
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends openssh-server \
    && echo "root:Docker!" | chpasswd
COPY sshd_config /etc/ssh/
EXPOSE 2222 80

COPY init_container.sh /app

RUN chmod 755 /app/init_container.sh

ENTRYPOINT ["/app/init_container.sh"]