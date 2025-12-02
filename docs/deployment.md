# Deployment Guide

## Overview

This guide covers deployment procedures for the Junior Golf App across different environments using Docker, Kubernetes, and CI/CD pipelines.

## Deployment Architecture

### Infrastructure Overview
```
Internet → Load Balancer → Web App (Next.js)
                      ↓
                   API Gateway → API Service (Node.js)
                              ↓
                           Database (PostgreSQL)
                              ↓
                           Cache (Redis)
```

### Environment Tiers
- **Development**: Local Docker containers
- **Staging**: AWS ECS with RDS
- **Production**: Kubernetes cluster with managed services

## Prerequisites

### Required Tools
- Docker & Docker Compose
- AWS CLI (configured)
- kubectl (for Kubernetes)
- Helm (for Kubernetes deployments)
- GitHub CLI (for CI/CD)

### AWS Services Setup
- **ECS/EKS**: Container orchestration
- **RDS**: PostgreSQL database
- **ElastiCache**: Redis caching
- **S3**: File storage
- **CloudFront**: CDN
- **Route 53**: DNS management
- **ALB**: Load balancing

## Local Development Deployment

### Docker Compose Setup
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Rebuild and start
docker-compose up --build -d
```

### Individual Service Deployment
```bash
# Build and run API service
cd services/api
docker build -t junior-golf-api .
docker run -p 3001:3001 --env-file .env.local junior-golf-api

# Build and run web app
cd apps/web
docker build -t junior-golf-web .
docker run -p 3000:3000 junior-golf-web
```

## Staging Deployment

### AWS ECS Deployment

#### Task Definition
```json
{
  "family": "junior-golf-api-staging",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "512",
  "memory": "1024",
  "executionRoleArn": "arn:aws:iam::account:role/ecsTaskExecutionRole",
  "containerDefinitions": [
    {
      "name": "api",
      "image": "your-account.dkr.ecr.region.amazonaws.com/junior-golf-api:staging",
      "portMappings": [
        {
          "containerPort": 3001,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "NODE_ENV",
          "value": "staging"
        }
      ],
      "secrets": [
        {
          "name": "DATABASE_URL",
          "valueFrom": "arn:aws:secretsmanager:region:account:secret:staging/database-url"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/junior-golf-api-staging",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
}
```

#### Service Definition
```json
{
  "serviceName": "junior-golf-api-staging",
  "cluster": "junior-golf-staging",
  "taskDefinition": "junior-golf-api-staging",
  "desiredCount": 2,
  "launchType": "FARGATE",
  "networkConfiguration": {
    "awsvpcConfiguration": {
      "subnets": ["subnet-12345", "subnet-67890"],
      "securityGroups": ["sg-api-staging"],
      "assignPublicIp": "ENABLED"
    }
  },
  "loadBalancers": [
    {
      "targetGroupArn": "arn:aws:elasticloadbalancing:region:account:targetgroup/api-staging",
      "containerName": "api",
      "containerPort": 3001
    }
  ]
}
```

### Deployment Script
```bash
#!/bin/bash
# deploy-staging.sh

set -e

# Build and push Docker image
docker build -t junior-golf-api:staging ./services/api
docker tag junior-golf-api:staging $ECR_REGISTRY/junior-golf-api:staging
docker push $ECR_REGISTRY/junior-golf-api:staging

# Update ECS service
aws ecs update-service \
  --cluster junior-golf-staging \
  --service junior-golf-api-staging \
  --force-new-deployment

# Wait for deployment to complete
aws ecs wait services-stable \
  --cluster junior-golf-staging \
  --services junior-golf-api-staging

echo "Staging deployment completed successfully"
```

## Production Deployment

### Kubernetes Configuration

#### Namespace
```yaml
# namespace.yaml
apiVersion: v1
kind: Namespace
metadata:
  name: junior-golf-prod
  labels:
    name: junior-golf-prod
```

#### ConfigMap
```yaml
# configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
  namespace: junior-golf-prod
data:
  NODE_ENV: "production"
  PORT: "3001"
  LOG_LEVEL: "warn"
  CORS_ORIGIN: "https://juniorgolf.ke"
```

#### Secrets
```yaml
# secrets.yaml
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
  namespace: junior-golf-prod
type: Opaque
data:
  DATABASE_URL: <base64-encoded-database-url>
  JWT_SECRET: <base64-encoded-jwt-secret>
  STRIPE_SECRET_KEY: <base64-encoded-stripe-key>
  REDIS_URL: <base64-encoded-redis-url>
```

#### API Deployment
```yaml
# api-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: junior-golf-api
  namespace: junior-golf-prod
spec:
  replicas: 3
  selector:
    matchLabels:
      app: junior-golf-api
  template:
    metadata:
      labels:
        app: junior-golf-api
    spec:
      containers:
      - name: api
        image: your-registry/junior-golf-api:latest
        ports:
        - containerPort: 3001
        envFrom:
        - configMapRef:
            name: app-config
        - secretRef:
            name: app-secrets
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 3001
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /ready
            port: 3001
          initialDelaySeconds: 5
          periodSeconds: 5
```

#### Service
```yaml
# api-service.yaml
apiVersion: v1
kind: Service
metadata:
  name: junior-golf-api-service
  namespace: junior-golf-prod
spec:
  selector:
    app: junior-golf-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 3001
  type: ClusterIP
```

#### Ingress
```yaml
# ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: junior-golf-ingress
  namespace: junior-golf-prod
  annotations:
    kubernetes.io/ingress.class: "nginx"
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
spec:
  tls:
  - hosts:
    - api.juniorgolf.ke
    - juniorgolf.ke
    secretName: junior-golf-tls
  rules:
  - host: api.juniorgolf.ke
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: junior-golf-api-service
            port:
              number: 80
  - host: juniorgolf.ke
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: junior-golf-web-service
            port:
              number: 80
```

### Helm Chart Deployment
```bash
# Install/upgrade with Helm
helm upgrade --install junior-golf-app ./helm/junior-golf \
  --namespace junior-golf-prod \
  --values ./helm/junior-golf/values.prod.yaml \
  --set image.tag=$BUILD_NUMBER
```

#### Helm Values (values.prod.yaml)
```yaml
# values.prod.yaml
replicaCount: 3

image:
  repository: your-registry/junior-golf-api
  tag: latest
  pullPolicy: Always

service:
  type: ClusterIP
  port: 80

ingress:
  enabled: true
  className: "nginx"
  annotations:
    cert-manager.io/cluster-issuer: letsencrypt-prod
  hosts:
    - host: api.juniorgolf.ke
      paths:
        - path: /
          pathType: Prefix
  tls:
    - secretName: junior-golf-tls
      hosts:
        - api.juniorgolf.ke

resources:
  limits:
    cpu: 500m
    memory: 1Gi
  requests:
    cpu: 250m
    memory: 512Mi

autoscaling:
  enabled: true
  minReplicas: 3
  maxReplicas: 10
  targetCPUUtilizationPercentage: 80

nodeSelector: {}
tolerations: []
affinity: {}
```

## CI/CD Pipeline

### GitHub Actions Workflow
```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [main]
  workflow_dispatch:

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
    - name: Checkout
      uses: actions/checkout@v4

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3

    - name: Log in to Container Registry
      uses: docker/login-action@v3
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}

    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}

    - name: Build and push API image
      uses: docker/build-push-action@v5
      with:
        context: ./services/api
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}

    - name: Build and push Web image
      uses: docker/build-push-action@v5
      with:
        context: ./apps/web
        push: true
        tags: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}-web:${{ github.sha }}

    - name: Configure kubectl
      uses: azure/k8s-set-context@v3
      with:
        method: kubeconfig
        kubeconfig: ${{ secrets.KUBE_CONFIG }}

    - name: Deploy to Kubernetes
      run: |
        helm upgrade --install junior-golf-app ./helm/junior-golf \
          --namespace junior-golf-prod \
          --values ./helm/junior-golf/values.prod.yaml \
          --set image.tag=${{ github.sha }} \
          --wait

    - name: Verify deployment
      run: |
        kubectl rollout status deployment/junior-golf-api -n junior-golf-prod
        kubectl get services -n junior-golf-prod
```

## Database Migrations

### Production Migration Strategy
```bash
#!/bin/bash
# migrate-production.sh

set -e

echo "Starting production database migration..."

# Create backup
kubectl exec -n junior-golf-prod deployment/junior-golf-api -- \
  npm run db:backup --workspace=services/database

# Run migrations
kubectl exec -n junior-golf-prod deployment/junior-golf-api -- \
  npm run migrate:deploy --workspace=services/database

# Verify migration
kubectl exec -n junior-golf-prod deployment/junior-golf-api -- \
  npm run db:status --workspace=services/database

echo "Migration completed successfully"
```

### Rollback Procedure
```bash
#!/bin/bash
# rollback.sh

set -e

PREVIOUS_VERSION=$1

if [ -z "$PREVIOUS_VERSION" ]; then
  echo "Usage: ./rollback.sh <previous-version>"
  exit 1
fi

echo "Rolling back to version: $PREVIOUS_VERSION"

# Rollback Kubernetes deployment
helm rollback junior-golf-app --namespace junior-golf-prod

# Rollback database if needed
kubectl exec -n junior-golf-prod deployment/junior-golf-api -- \
  npm run db:rollback --workspace=services/database

echo "Rollback completed"
```

## Monitoring & Health Checks

### Health Check Endpoints
```typescript
// health.ts
app.get('/health', async (req, res) => {
  const health = {
    status: 'healthy',
    timestamp: new Date().toISOString(),
    uptime: process.uptime(),
    checks: {
      database: await checkDatabase(),
      redis: await checkRedis(),
      external_apis: await checkExternalAPIs()
    }
  };
  
  const isHealthy = Object.values(health.checks)
    .every(check => check.status === 'healthy');
  
  res.status(isHealthy ? 200 : 503).json(health);
});
```

### Kubernetes Probes
```yaml
livenessProbe:
  httpGet:
    path: /health
    port: 3001
  initialDelaySeconds: 30
  periodSeconds: 10
  timeoutSeconds: 5
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /ready
    port: 3001
  initialDelaySeconds: 5
  periodSeconds: 5
  timeoutSeconds: 3
  failureThreshold: 3
```

## Security Considerations

### Image Security
```dockerfile
# Use non-root user
FROM node:18-alpine
RUN addgroup -g 1001 -S nodejs
RUN adduser -S nextjs -u 1001
USER nextjs

# Scan for vulnerabilities
RUN npm audit --audit-level high
```

### Network Security
```yaml
# NetworkPolicy
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: junior-golf-network-policy
  namespace: junior-golf-prod
spec:
  podSelector:
    matchLabels:
      app: junior-golf-api
  policyTypes:
  - Ingress
  - Egress
  ingress:
  - from:
    - namespaceSelector:
        matchLabels:
          name: ingress-nginx
    ports:
    - protocol: TCP
      port: 3001
```

## Troubleshooting

### Common Deployment Issues

#### Pod Startup Failures
```bash
# Check pod status
kubectl get pods -n junior-golf-prod

# View pod logs
kubectl logs -f deployment/junior-golf-api -n junior-golf-prod

# Describe pod for events
kubectl describe pod <pod-name> -n junior-golf-prod
```

#### Database Connection Issues
```bash
# Test database connectivity
kubectl exec -it deployment/junior-golf-api -n junior-golf-prod -- \
  npm run db:test --workspace=services/database

# Check secrets
kubectl get secrets -n junior-golf-prod
kubectl describe secret app-secrets -n junior-golf-prod
```

#### Performance Issues
```bash
# Check resource usage
kubectl top pods -n junior-golf-prod

# Scale deployment
kubectl scale deployment junior-golf-api --replicas=5 -n junior-golf-prod

# Check HPA status
kubectl get hpa -n junior-golf-prod
```