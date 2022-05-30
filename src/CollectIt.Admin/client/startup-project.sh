#!/usr/bin/sh

export PORT

envsubst '{$PORT}' </nginx.conf.template >/etc/nginx/nginx.conf
cat /etc/nginx/nginx.conf
nginx -g 'daemon off;'