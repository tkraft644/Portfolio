(() => {
  const STORAGE_KEY = 'portfolio_theme';
  const root = document.documentElement;

  function isTheme(value) {
    return value === 'light' || value === 'dark';
  }

  function readStoredTheme() {
    try {
      const value = localStorage.getItem(STORAGE_KEY);
      return isTheme(value) ? value : null;
    } catch {
      return null;
    }
  }

  function getSystemTheme() {
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }

  function getEffectiveTheme() {
    const explicit = root.dataset.theme;
    if (isTheme(explicit)) {
      return explicit;
    }

    const stored = readStoredTheme();
    if (stored) {
      return stored;
    }

    return getSystemTheme();
  }

  function applyTheme(theme) {
    root.dataset.theme = theme;
  }

  function setTheme(theme) {
    try {
      localStorage.setItem(STORAGE_KEY, theme);
    } catch {
      // ignore
    }
    applyTheme(theme);
  }

  function toggleTheme() {
    const current = getEffectiveTheme();
    const next = current === 'dark' ? 'light' : 'dark';
    setTheme(next);
    updateToggles();
  }

  function updateToggles() {
    const theme = getEffectiveTheme();

    document.querySelectorAll('[data-theme-toggle]').forEach((el) => {
      const btn = /** @type {HTMLElement} */ (el);

      const darkLabel = btn.getAttribute('data-theme-label-dark') || 'Dark';
      const lightLabel = btn.getAttribute('data-theme-label-light') || 'Light';

      const label = theme === 'dark' ? darkLabel : lightLabel;
      const labelEl = btn.querySelector('[data-theme-toggle-label]');
      if (labelEl) {
        labelEl.textContent = label;
      }

      btn.setAttribute('aria-pressed', theme === 'dark' ? 'true' : 'false');
      btn.setAttribute('aria-label', label);
      btn.setAttribute('title', label);
    });
  }

  // Apply stored theme early (if present); otherwise keep system default via CSS.
  const stored = readStoredTheme();
  if (stored) {
    applyTheme(stored);
  }

  document.addEventListener('click', (ev) => {
    const target = ev.target instanceof Element ? ev.target.closest('[data-theme-toggle]') : null;
    if (!target) {
      return;
    }

    ev.preventDefault();
    toggleTheme();
  });

  if (window.matchMedia) {
    const mql = window.matchMedia('(prefers-color-scheme: dark)');
    if (typeof mql.addEventListener === 'function') {
      mql.addEventListener('change', () => {
        if (!readStoredTheme() && !root.dataset.theme) {
          updateToggles();
        }
      });
    }
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', updateToggles);
  } else {
    updateToggles();
  }
})();

