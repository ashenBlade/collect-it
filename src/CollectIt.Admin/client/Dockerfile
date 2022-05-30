FROM node:18.0.0-alpine AS BUILD

WORKDIR /app
COPY package*.json ./
COPY ./ ./
RUN npm i
ENV SERVER_ADDRESS="https://collect-it-backend"
RUN npm run build

FROM nginx:1.21-alpine AS PROD

WORKDIR /app

RUN apk add gettext
COPY --from=BUILD /app/build/ /www/content
COPY nginx.conf.template startup-project.sh /

ENTRYPOINT ["/bin/sh", "/startup-project.sh"]

