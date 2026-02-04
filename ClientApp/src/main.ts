import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';

// Apply stored theme early (before Angular bootstraps).
try {
  const stored = localStorage.getItem('portfolio_theme');
  if (stored === 'light' || stored === 'dark') {
    document.documentElement.dataset['theme'] = stored;
  }
} catch {
  // ignore
}

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
