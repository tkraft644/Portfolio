import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

type Theme = 'light' | 'dark';

const THEME_STORAGE_KEY = 'portfolio_theme';

function getSystemTheme(): Theme {
  return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

function readStoredTheme(): Theme | null {
  try {
    const value = localStorage.getItem(THEME_STORAGE_KEY);
    return value === 'light' || value === 'dark' ? value : null;
  } catch {
    return null;
  }
}

function getEffectiveTheme(): Theme {
  const explicit = document.documentElement.dataset['theme'];
  if (explicit === 'light' || explicit === 'dark') {
    return explicit;
  }

  return readStoredTheme() ?? getSystemTheme();
}

function setTheme(theme: Theme) {
  try {
    localStorage.setItem(THEME_STORAGE_KEY, theme);
  } catch {
    // ignore
  }

  document.documentElement.dataset['theme'] = theme;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected theme: Theme = getEffectiveTheme();

  protected toggleTheme() {
    this.theme = this.theme === 'dark' ? 'light' : 'dark';
    setTheme(this.theme);
  }
}
