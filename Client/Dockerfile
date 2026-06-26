# ---- Build Stage ----
FROM cgr.dev/chainguard/node:latest-dev AS build

WORKDIR /app

COPY --chown=node:node package.json package-lock.json ./
RUN npm ci

COPY --chown=node:node . .

# API istekleri nginx reverse proxy uzerinden gidecek
# /api path'i nginx tarafindan backend'e yonlendirilecek
ENV VITE_API_URL=/api

RUN npm run build

# ---- Production Stage ----
FROM cgr.dev/chainguard/nginx:latest AS final

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 8080
