# Monitoring & Observability

## Overview

This document covers monitoring, logging, and observability strategies for the Junior Golf App to ensure system reliability and performance.

## Monitoring Stack

### Core Components
- **Metrics**: Prometheus + Grafana
- **Logging**: ELK Stack (Elasticsearch, Logstash, Kibana)
- **Tracing**: Jaeger
- **Error Tracking**: Sentry
- **Uptime Monitoring**: Pingdom/UptimeRobot
- **APM**: New Relic/DataDog (optional)

### Infrastructure Monitoring
- **Kubernetes**: Prometheus Operator
- **AWS Services**: CloudWatch
- **Database**: PostgreSQL metrics
- **Cache**: Redis monitoring

## Application Metrics

### Key Performance Indicators (KPIs)

#### Business Metrics
- **User Registrations**: Daily/weekly/monthly signups
- **Active Users**: DAU/WAU/MAU
- **Event Registrations**: Conversion rates
- **Payment Success Rate**: Transaction completion
- **Member Retention**: Churn analysis

#### Technical Metrics
- **Response Time**: API endpoint latency
- **Throughput**: Requests per second
- **Error Rate**: 4xx/5xx error percentage
- **Availability**: Uptime percentage
- **Database Performance**: Query execution time

### Prometheus Configuration

#### Metrics Collection
```yaml
# prometheus.yml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

rule_files:
  - "alert_rules.yml"

scrape_configs:
  - job_name: 'junior-golf-api'
    static_configs:
      - targets: ['api:3001']
    metrics_path: '/metrics'
    scrape_interval: 10s

  - job_name: 'kubernetes-pods'
    kubernetes_sd_configs:
      - role: pod
    relabel_configs:
      - source_labels: [__meta_kubernetes_pod_annotation_prometheus_io_scrape]
        action: keep
        regex: true

alerting:
  alertmanagers:
    - static_configs:
        - targets:
          - alertmanager:9093
```

#### Custom Metrics Implementation
```typescript
// metrics.ts
import client from 'prom-client';

// Create a Registry
const register = new client.Registry();

// Default metrics
client.collectDefaultMetrics({ register });

// Custom metrics
export const httpRequestDuration = new client.Histogram({
  name: 'http_request_duration_seconds',
  help: 'Duration of HTTP requests in seconds',
  labelNames: ['method', 'route', 'status_code'],
  buckets: [0.1, 0.3, 0.5, 0.7, 1, 3, 5, 7, 10]
});

export const httpRequestTotal = new client.Counter({
  name: 'http_requests_total',
  help: 'Total number of HTTP requests',
  labelNames: ['method', 'route', 'status_code']
});

export const activeUsers = new client.Gauge({
  name: 'active_users_total',
  help: 'Number of currently active users'
});

export const databaseConnections = new client.Gauge({
  name: 'database_connections_active',
  help: 'Number of active database connections'
});

// Register metrics
register.registerMetric(httpRequestDuration);
register.registerMetric(httpRequestTotal);
register.registerMetric(activeUsers);
register.registerMetric(databaseConnections);

// Middleware for Express
export const metricsMiddleware = (req: Request, res: Response, next: NextFunction) => {
  const start = Date.now();
  
  res.on('finish', () => {
    const duration = (Date.now() - start) / 1000;
    const route = req.route?.path || req.path;
    
    httpRequestDuration
      .labels(req.method, route, res.statusCode.toString())
      .observe(duration);
    
    httpRequestTotal
      .labels(req.method, route, res.statusCode.toString())
      .inc();
  });
  
  next();
};

// Metrics endpoint
export const metricsHandler = async (req: Request, res: Response) => {
  res.set('Content-Type', register.contentType);
  res.end(await register.metrics());
};
```

## Logging Strategy

### Log Levels
- **ERROR**: System errors, exceptions
- **WARN**: Warning conditions
- **INFO**: General information
- **DEBUG**: Detailed debugging information

### Structured Logging
```typescript
// logger.ts
import winston from 'winston';

const logger = winston.createLogger({
  level: process.env.LOG_LEVEL || 'info',
  format: winston.format.combine(
    winston.format.timestamp(),
    winston.format.errors({ stack: true }),
    winston.format.json()
  ),
  defaultMeta: {
    service: 'junior-golf-api',
    version: process.env.APP_VERSION
  },
  transports: [
    new winston.transports.Console({
      format: winston.format.combine(
        winston.format.colorize(),
        winston.format.simple()
      )
    }),
    new winston.transports.File({
      filename: 'logs/error.log',
      level: 'error'
    }),
    new winston.transports.File({
      filename: 'logs/combined.log'
    })
  ]
});

// Request logging middleware
export const requestLogger = (req: Request, res: Response, next: NextFunction) => {
  const start = Date.now();
  
  res.on('finish', () => {
    const duration = Date.now() - start;
    
    logger.info('HTTP Request', {
      method: req.method,
      url: req.url,
      statusCode: res.statusCode,
      duration,
      userAgent: req.get('User-Agent'),
      ip: req.ip,
      userId: req.user?.id
    });
  });
  
  next();
};

export default logger;
```

### ELK Stack Configuration

#### Logstash Configuration
```ruby
# logstash.conf
input {
  beats {
    port => 5044
  }
}

filter {
  if [fields][service] == "junior-golf-api" {
    json {
      source => "message"
    }
    
    date {
      match => [ "timestamp", "ISO8601" ]
    }
    
    mutate {
      add_field => { "environment" => "${ENVIRONMENT:development}" }
    }
  }
}

output {
  elasticsearch {
    hosts => ["elasticsearch:9200"]
    index => "junior-golf-%{+YYYY.MM.dd}"
  }
}
```

#### Filebeat Configuration
```yaml
# filebeat.yml
filebeat.inputs:
- type: log
  enabled: true
  paths:
    - /var/log/junior-golf/*.log
  fields:
    service: junior-golf-api
  fields_under_root: true

output.logstash:
  hosts: ["logstash:5044"]

processors:
  - add_host_metadata:
      when.not.contains.tags: forwarded
```

## Alerting Rules

### Prometheus Alert Rules
```yaml
# alert_rules.yml
groups:
- name: junior-golf-alerts
  rules:
  - alert: HighErrorRate
    expr: rate(http_requests_total{status_code=~"5.."}[5m]) > 0.1
    for: 5m
    labels:
      severity: critical
    annotations:
      summary: "High error rate detected"
      description: "Error rate is {{ $value }} errors per second"

  - alert: HighResponseTime
    expr: histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m])) > 1
    for: 5m
    labels:
      severity: warning
    annotations:
      summary: "High response time detected"
      description: "95th percentile response time is {{ $value }} seconds"

  - alert: DatabaseConnectionsHigh
    expr: database_connections_active > 80
    for: 2m
    labels:
      severity: warning
    annotations:
      summary: "High database connection usage"
      description: "Database connections: {{ $value }}"

  - alert: ServiceDown
    expr: up == 0
    for: 1m
    labels:
      severity: critical
    annotations:
      summary: "Service is down"
      description: "{{ $labels.instance }} has been down for more than 1 minute"
```

### Alertmanager Configuration
```yaml
# alertmanager.yml
global:
  smtp_smarthost: 'localhost:587'
  smtp_from: 'alerts@juniorgolf.ke'

route:
  group_by: ['alertname']
  group_wait: 10s
  group_interval: 10s
  repeat_interval: 1h
  receiver: 'web.hook'

receivers:
- name: 'web.hook'
  email_configs:
  - to: 'admin@juniorgolf.ke'
    subject: 'Junior Golf Alert: {{ .GroupLabels.alertname }}'
    body: |
      {{ range .Alerts }}
      Alert: {{ .Annotations.summary }}
      Description: {{ .Annotations.description }}
      {{ end }}
  
  slack_configs:
  - api_url: 'YOUR_SLACK_WEBHOOK_URL'
    channel: '#alerts'
    title: 'Junior Golf Alert'
    text: '{{ range .Alerts }}{{ .Annotations.summary }}{{ end }}'
```

## Grafana Dashboards

### System Overview Dashboard
```json
{
  "dashboard": {
    "title": "Junior Golf App - System Overview",
    "panels": [
      {
        "title": "Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_total[5m])",
            "legendFormat": "{{ method }} {{ route }}"
          }
        ]
      },
      {
        "title": "Response Time",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))",
            "legendFormat": "95th percentile"
          }
        ]
      },
      {
        "title": "Error Rate",
        "type": "singlestat",
        "targets": [
          {
            "expr": "rate(http_requests_total{status_code=~\"5..\"}[5m])",
            "legendFormat": "5xx errors/sec"
          }
        ]
      }
    ]
  }
}
```

### Business Metrics Dashboard
```json
{
  "dashboard": {
    "title": "Junior Golf App - Business Metrics",
    "panels": [
      {
        "title": "Daily Active Users",
        "type": "graph",
        "targets": [
          {
            "expr": "active_users_total",
            "legendFormat": "Active Users"
          }
        ]
      },
      {
        "title": "Registration Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(user_registrations_total[1h])",
            "legendFormat": "Registrations/hour"
          }
        ]
      },
      {
        "title": "Payment Success Rate",
        "type": "singlestat",
        "targets": [
          {
            "expr": "rate(payments_successful_total[5m]) / rate(payments_total[5m]) * 100",
            "legendFormat": "Success Rate %"
          }
        ]
      }
    ]
  }
}
```

## Error Tracking

### Sentry Integration
```typescript
// sentry.ts
import * as Sentry from '@sentry/node';
import { ProfilingIntegration } from '@sentry/profiling-node';

Sentry.init({
  dsn: process.env.SENTRY_DSN,
  environment: process.env.NODE_ENV,
  integrations: [
    new ProfilingIntegration(),
  ],
  tracesSampleRate: 1.0,
  profilesSampleRate: 1.0,
});

// Error handling middleware
export const sentryErrorHandler = (error: Error, req: Request, res: Response, next: NextFunction) => {
  Sentry.captureException(error, {
    user: {
      id: req.user?.id,
      email: req.user?.email
    },
    extra: {
      url: req.url,
      method: req.method,
      body: req.body,
      query: req.query
    }
  });
  
  next(error);
};
```

## Health Checks

### Application Health Endpoints
```typescript
// health.ts
interface HealthCheck {
  status: 'healthy' | 'unhealthy';
  message?: string;
  details?: any;
}

interface HealthStatus {
  status: 'healthy' | 'unhealthy';
  timestamp: string;
  uptime: number;
  version: string;
  checks: {
    database: HealthCheck;
    redis: HealthCheck;
    external_apis: HealthCheck;
  };
}

export const healthChecks = {
  database: async (): Promise<HealthCheck> => {
    try {
      await prisma.$queryRaw`SELECT 1`;
      return { status: 'healthy' };
    } catch (error) {
      return {
        status: 'unhealthy',
        message: 'Database connection failed',
        details: error.message
      };
    }
  },

  redis: async (): Promise<HealthCheck> => {
    try {
      await redis.ping();
      return { status: 'healthy' };
    } catch (error) {
      return {
        status: 'unhealthy',
        message: 'Redis connection failed',
        details: error.message
      };
    }
  },

  external_apis: async (): Promise<HealthCheck> => {
    try {
      // Check critical external services
      const checks = await Promise.allSettled([
        fetch('https://api.stripe.com/v1/charges', { method: 'HEAD' }),
        // Add other external API checks
      ]);

      const failures = checks.filter(check => check.status === 'rejected');
      
      if (failures.length > 0) {
        return {
          status: 'unhealthy',
          message: 'External API checks failed',
          details: failures
        };
      }

      return { status: 'healthy' };
    } catch (error) {
      return {
        status: 'unhealthy',
        message: 'External API health check failed',
        details: error.message
      };
    }
  }
};

export const getHealthStatus = async (): Promise<HealthStatus> => {
  const checks = await Promise.all([
    healthChecks.database(),
    healthChecks.redis(),
    healthChecks.external_apis()
  ]);

  const [database, redis, external_apis] = checks;
  
  const isHealthy = checks.every(check => check.status === 'healthy');

  return {
    status: isHealthy ? 'healthy' : 'unhealthy',
    timestamp: new Date().toISOString(),
    uptime: process.uptime(),
    version: process.env.APP_VERSION || '1.0.0',
    checks: {
      database,
      redis,
      external_apis
    }
  };
};
```

## Performance Monitoring

### Database Query Monitoring
```typescript
// database-monitoring.ts
import { PrismaClient } from '@prisma/client';

const prisma = new PrismaClient({
  log: [
    {
      emit: 'event',
      level: 'query',
    },
  ],
});

prisma.$on('query', (e) => {
  if (e.duration > 1000) { // Log slow queries (>1s)
    logger.warn('Slow query detected', {
      query: e.query,
      duration: e.duration,
      params: e.params
    });
  }
  
  // Update metrics
  databaseQueryDuration
    .labels(e.query.split(' ')[0]) // First word (SELECT, INSERT, etc.)
    .observe(e.duration / 1000);
});
```

### Memory and CPU Monitoring
```typescript
// system-monitoring.ts
import { performance, PerformanceObserver } from 'perf_hooks';

// Monitor event loop lag
const eventLoopLag = new client.Histogram({
  name: 'nodejs_eventloop_lag_seconds',
  help: 'Lag of event loop in seconds',
  buckets: [0.001, 0.01, 0.1, 1, 10]
});

setInterval(() => {
  const start = performance.now();
  setImmediate(() => {
    const lag = (performance.now() - start) / 1000;
    eventLoopLag.observe(lag);
  });
}, 5000);

// Monitor memory usage
const memoryUsage = new client.Gauge({
  name: 'nodejs_memory_usage_bytes',
  help: 'Memory usage in bytes',
  labelNames: ['type']
});

setInterval(() => {
  const usage = process.memoryUsage();
  memoryUsage.labels('rss').set(usage.rss);
  memoryUsage.labels('heapUsed').set(usage.heapUsed);
  memoryUsage.labels('heapTotal').set(usage.heapTotal);
  memoryUsage.labels('external').set(usage.external);
}, 10000);
```

## Monitoring Best Practices

### Alerting Guidelines
1. **Alert on symptoms, not causes**
2. **Use appropriate thresholds**
3. **Avoid alert fatigue**
4. **Include actionable information**
5. **Test alert channels regularly**

### Dashboard Design
1. **Start with business metrics**
2. **Use consistent time ranges**
3. **Group related metrics**
4. **Include context and annotations**
5. **Make dashboards discoverable**

### Log Management
1. **Use structured logging**
2. **Include correlation IDs**
3. **Log at appropriate levels**
4. **Sanitize sensitive data**
5. **Set up log retention policies**