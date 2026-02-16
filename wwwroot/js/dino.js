(function () {
  const canvas = document.getElementById('dino-canvas');
  if (!(canvas instanceof HTMLCanvasElement)) {
    return;
  }

  const ctx = canvas.getContext('2d');
  if (!ctx) {
    return;
  }

  const container = canvas.closest('.dino-canvas-container');
  if (!(container instanceof HTMLElement)) {
    return;
  }

  const overlay = container.querySelector('[data-dino-overlay]');
  const overlaySubtitle = container.querySelector('.dino-overlay-subtitle');

  const scoreEl = document.querySelector('[data-dino-score]');
  const bestEl = document.querySelector('[data-dino-best]');
  const restartBtn = document.querySelector('[data-dino-restart]');

  const apiConfig = document.querySelector('[data-dino-api-config]');
  const leaderboardBody = document.querySelector('[data-dino-leaderboard-body]');
  const leaderboardStatusEl = document.querySelector('[data-dino-leaderboard-status]');

  const recordModal = document.querySelector('[data-dino-record-modal]');
  const recordModalText = recordModal?.querySelector('[data-dino-record-modal-text]');
  const recordForm = recordModal?.querySelector('[data-dino-record-form]');
  const recordNameInput = recordModal?.querySelector('[data-dino-record-name]');
  const recordValidation = recordModal?.querySelector('[data-dino-record-validation]');
  const recordSaveBtn = recordModal?.querySelector('[data-dino-record-save]');

  const WORLD_WIDTH = 900;
  const WORLD_HEIGHT = 240;
  const GROUND_Y = 190;
  const LEADERBOARD_LIMIT = 10;

  const STORAGE_KEY_BEST = 'portfolio_dino_best';
  const STORAGE_KEY_PLAYER_NAME = 'portfolio_dino_player_name';

  const leaderboardUrl = (apiConfig?.getAttribute('data-leaderboard-url') || '/api/fun/leaderboard').trim();
  const submitUrl = (apiConfig?.getAttribute('data-submit-url') || '/api/fun/leaderboard').trim();

  const leaderboardLoadingText = leaderboardBody?.getAttribute('data-loading') || 'Loading results...';
  const leaderboardEmptyText = leaderboardBody?.getAttribute('data-empty') || 'No records yet.';
  const leaderboardErrorText = leaderboardBody?.getAttribute('data-error') || 'Could not load leaderboard.';

  const recordPromptText = recordModal?.getAttribute('data-prompt') || 'Add your score to leaderboard?';
  const recordScorePrefix = recordModal?.getAttribute('data-score-prefix') || 'Score';
  const recordValidationText = recordModal?.getAttribute('data-validation') || 'Enter at least 2 characters.';
  const recordSaveErrorText = recordModal?.getAttribute('data-save-error') || 'Could not save score. Try again.';
  const recordSaveSuccessText = recordModal?.getAttribute('data-save-success') || 'Score saved in leaderboard.';
  const recordSaveButtonLabel = recordModal?.getAttribute('data-save-label') || 'Save';
  const recordSavingButtonLabel = recordModal?.getAttribute('data-saving-label') || 'Saving...';

  const DINO = {
    x: 92,
    w: 34,
    h: 46
  };

  const PHYSICS = {
    gravity: 2300,
    jumpVelocity: -760
  };

  const SPEED = {
    base: 360,
    ramp: 0.85,
    max: 820
  };

  const SPAWN = {
    min: 0.85,
    max: 1.35
  };

  let transformScale = 1;
  let dpr = 1;

  /** @type {{text:string,accent:string,border:string}|null} */
  let themeColors = null;

  /** @type {bootstrap.Modal | null} */
  let recordModalInstance = null;

  let isModalOpen = false;
  let isSubmittingScore = false;
  let pendingScoreToSubmit = null;

  function clamp(value, min, max) {
    return Math.min(max, Math.max(min, value));
  }

  function rand(min, max) {
    return Math.random() * (max - min) + min;
  }

  function readBest() {
    try {
      const raw = localStorage.getItem(STORAGE_KEY_BEST);
      const parsed = raw ? Number(raw) : 0;
      return Number.isFinite(parsed) ? Math.max(0, Math.floor(parsed)) : 0;
    } catch {
      return 0;
    }
  }

  function writeBest(value) {
    try {
      localStorage.setItem(STORAGE_KEY_BEST, String(Math.floor(value)));
    } catch {
      // ignore
    }
  }

  function readPlayerName() {
    try {
      return (localStorage.getItem(STORAGE_KEY_PLAYER_NAME) || '').trim();
    } catch {
      return '';
    }
  }

  function writePlayerName(value) {
    try {
      localStorage.setItem(STORAGE_KEY_PLAYER_NAME, value);
    } catch {
      // ignore
    }
  }

  function readThemeColors() {
    const styles = getComputedStyle(document.documentElement);
    const text = styles.getPropertyValue('--text').trim();
    const accent = styles.getPropertyValue('--accent').trim();
    const border = styles.getPropertyValue('--panel-border').trim();

    return {
      text: text || '#d4d4d4',
      accent: accent || '#007acc',
      border: border || '#3f3f46'
    };
  }

  function applyOverlayText(mode) {
    if (!(overlay instanceof HTMLElement) || !(overlaySubtitle instanceof HTMLElement)) {
      return;
    }

    const startText = overlaySubtitle.getAttribute('data-dino-start') || '';
    const gameOverText = overlaySubtitle.getAttribute('data-dino-gameover') || '';
    const pausedText = overlaySubtitle.getAttribute('data-dino-paused') || '';

    if (mode === 'start') {
      overlaySubtitle.textContent = startText;
      overlay.hidden = false;
      return;
    }

    if (mode === 'gameover') {
      overlaySubtitle.textContent = gameOverText;
      overlay.hidden = false;
      return;
    }

    if (mode === 'paused') {
      overlaySubtitle.textContent = pausedText;
      overlay.hidden = false;
      return;
    }

    overlay.hidden = true;
  }

  function isTypingTarget(target) {
    return (
      target instanceof HTMLInputElement ||
      target instanceof HTMLTextAreaElement ||
      (target instanceof HTMLElement && target.isContentEditable)
    );
  }

  function resizeCanvas() {
    const rect = canvas.getBoundingClientRect();
    const cssWidth = Math.max(1, Math.round(rect.width));
    const cssHeight = Math.max(1, Math.round(rect.height));

    dpr = window.devicePixelRatio || 1;

    canvas.width = Math.round(cssWidth * dpr);
    canvas.height = Math.round(cssHeight * dpr);

    transformScale = cssWidth / WORLD_WIDTH;
    ctx.setTransform(dpr * transformScale, 0, 0, dpr * transformScale, 0, 0);
  }

  function rectsIntersect(a, b) {
    return a.x < b.x + b.w && a.x + a.w > b.x && a.y < b.y + b.h && a.y + a.h > b.y;
  }

  function roundRect(x, y, w, h, r) {
    const radius = Math.min(r, w / 2, h / 2);
    ctx.beginPath();
    ctx.moveTo(x + radius, y);
    ctx.arcTo(x + w, y, x + w, y + h, radius);
    ctx.arcTo(x + w, y + h, x, y + h, radius);
    ctx.arcTo(x, y + h, x, y, radius);
    ctx.arcTo(x, y, x + w, y, radius);
    ctx.closePath();
  }

  function drawDino(state, stepPhase) {
    const colors = themeColors ?? (themeColors = readThemeColors());
    const x = DINO.x;
    const y = state.dinoY;

    ctx.fillStyle = colors.text;
    roundRect(x, y, DINO.w, DINO.h, 6);
    ctx.fill();

    ctx.fillStyle = colors.accent;
    ctx.fillRect(x + 23, y + 10, 4, 4);

    ctx.fillStyle = colors.text;
    const legY = y + DINO.h - 6;
    const legA = stepPhase ? 2 : 0;
    ctx.fillRect(x + 8 + legA, legY, 6, 6);
    ctx.fillRect(x + 20 - legA, legY, 6, 6);
  }

  function drawObstacle(ob) {
    const colors = themeColors ?? (themeColors = readThemeColors());
    ctx.fillStyle = colors.text;

    roundRect(ob.x, ob.y, ob.w, ob.h, 6);
    ctx.fill();

    if (ob.arm) {
      const armW = Math.max(10, Math.floor(ob.w * 0.55));
      const armH = Math.max(10, Math.floor(ob.h * 0.20));
      const armY = ob.y + Math.floor(ob.h * 0.35);
      const armX = ob.arm === 'left' ? ob.x - armW + 6 : ob.x + ob.w - 6;
      roundRect(armX, armY, armW, armH, 6);
      ctx.fill();
    }
  }

  function drawGround() {
    const colors = themeColors ?? (themeColors = readThemeColors());
    ctx.strokeStyle = colors.border;
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.moveTo(0, GROUND_Y + 0.5);
    ctx.lineTo(WORLD_WIDTH, GROUND_Y + 0.5);
    ctx.stroke();
  }

  function updateHud(score, best) {
    if (scoreEl instanceof HTMLElement) scoreEl.textContent = String(Math.floor(score));
    if (bestEl instanceof HTMLElement) bestEl.textContent = String(Math.floor(best));
  }

  function normalizePlayerName(rawValue) {
    if (typeof rawValue !== 'string') {
      return '';
    }

    const compact = rawValue
      .replace(/\s+/g, ' ')
      .trim()
      .slice(0, 80);

    return compact;
  }

  function setLeaderboardStatus(message, isError) {
    if (!(leaderboardStatusEl instanceof HTMLElement)) {
      return;
    }

    leaderboardStatusEl.textContent = message || '';
    leaderboardStatusEl.classList.toggle('text-danger', Boolean(isError));
    leaderboardStatusEl.classList.toggle('text-success', Boolean(message) && !isError);
  }

  function renderLeaderboardMessage(message) {
    if (!(leaderboardBody instanceof HTMLTableSectionElement)) {
      return;
    }

    const row = document.createElement('tr');
    const cell = document.createElement('td');
    cell.colSpan = 4;
    cell.className = 'text-center text-muted py-3';
    cell.textContent = message;
    row.appendChild(cell);

    leaderboardBody.innerHTML = '';
    leaderboardBody.appendChild(row);
  }

  function formatLeaderboardDate(value) {
    if (!value) {
      return '—';
    }

    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) {
      return '—';
    }

    return parsed.toLocaleString();
  }

  function renderLeaderboardRows(entries) {
    if (!(leaderboardBody instanceof HTMLTableSectionElement)) {
      return;
    }

    leaderboardBody.innerHTML = '';
    if (!Array.isArray(entries) || entries.length === 0) {
      renderLeaderboardMessage(leaderboardEmptyText);
      return;
    }

    for (let index = 0; index < entries.length; index += 1) {
      const entry = entries[index];
      const row = document.createElement('tr');

      const rankCell = document.createElement('td');
      rankCell.className = 'fw-semibold';
      rankCell.textContent = String(Number(entry.rank) || index + 1);

      const playerCell = document.createElement('td');
      playerCell.textContent = String(entry.playerName || '—');

      const scoreCell = document.createElement('td');
      scoreCell.className = 'text-end fw-semibold';
      scoreCell.textContent = String(Math.max(0, Math.floor(Number(entry.score) || 0)));

      const whenCell = document.createElement('td');
      whenCell.className = 'small text-muted';
      whenCell.textContent = formatLeaderboardDate(entry.createdAtUtc);

      row.append(rankCell, playerCell, scoreCell, whenCell);
      leaderboardBody.appendChild(row);
    }
  }

  function buildLeaderboardUrl(limit) {
    try {
      const url = new URL(leaderboardUrl, window.location.origin);
      url.searchParams.set('limit', String(limit));
      return url.toString();
    } catch {
      return `${leaderboardUrl}?limit=${encodeURIComponent(String(limit))}`;
    }
  }

  async function loadLeaderboard() {
    renderLeaderboardMessage(leaderboardLoadingText);

    try {
      const response = await fetch(buildLeaderboardUrl(LEADERBOARD_LIMIT), {
        method: 'GET',
        headers: { Accept: 'application/json' },
        cache: 'no-store'
      });

      if (!response.ok) {
        throw new Error(`Leaderboard HTTP ${response.status}`);
      }

      const entries = await response.json();
      renderLeaderboardRows(entries);
    } catch {
      renderLeaderboardMessage(leaderboardErrorText);
    }
  }

  function clearModalValidation() {
    if (recordNameInput instanceof HTMLElement) {
      recordNameInput.classList.remove('is-invalid');
    }
    if (recordValidation instanceof HTMLElement) {
      recordValidation.textContent = '';
    }
  }

  function showModalValidation(message) {
    if (recordNameInput instanceof HTMLElement) {
      recordNameInput.classList.add('is-invalid');
    }
    if (recordValidation instanceof HTMLElement) {
      recordValidation.textContent = message;
    }
  }

  function setRecordSaveBusy(isBusy) {
    isSubmittingScore = isBusy;
    if (recordSaveBtn instanceof HTMLButtonElement) {
      recordSaveBtn.disabled = isBusy;
      recordSaveBtn.textContent = isBusy ? recordSavingButtonLabel : recordSaveButtonLabel;
    }
  }

  async function postLeaderboardEntry(playerName, scoreValue) {
    const response = await fetch(submitUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json'
      },
      body: JSON.stringify({
        playerName,
        score: scoreValue
      })
    });

    if (!response.ok) {
      throw new Error(`Submit HTTP ${response.status}`);
    }
  }

  async function submitScoreFromModal() {
    if (isSubmittingScore || pendingScoreToSubmit === null) {
      return;
    }

    const nameValue = normalizePlayerName(recordNameInput instanceof HTMLInputElement ? recordNameInput.value : '');
    if (nameValue.length < 2) {
      showModalValidation(recordValidationText);
      return;
    }

    clearModalValidation();
    setRecordSaveBusy(true);

    try {
      await postLeaderboardEntry(nameValue, pendingScoreToSubmit);
      writePlayerName(nameValue);
      pendingScoreToSubmit = null;
      setLeaderboardStatus(recordSaveSuccessText, false);
      await loadLeaderboard();

      if (recordModalInstance) {
        recordModalInstance.hide();
      }
    } catch {
      setLeaderboardStatus(recordSaveErrorText, true);
      showModalValidation(recordSaveErrorText);
    } finally {
      setRecordSaveBusy(false);
    }
  }

  async function promptAndSubmitFallback(scoreValue) {
    const shouldSave = window.confirm(recordPromptText);
    if (!shouldSave) {
      return;
    }

    const suggestedName = readPlayerName();
    const rawName = window.prompt(`${recordScorePrefix}: ${scoreValue}`, suggestedName) || '';
    const normalized = normalizePlayerName(rawName);
    if (normalized.length < 2) {
      setLeaderboardStatus(recordValidationText, true);
      return;
    }

    try {
      await postLeaderboardEntry(normalized, scoreValue);
      writePlayerName(normalized);
      setLeaderboardStatus(recordSaveSuccessText, false);
      await loadLeaderboard();
    } catch {
      setLeaderboardStatus(recordSaveErrorText, true);
    }
  }

  function promptToSaveRecord(scoreValue) {
    const safeScore = Math.max(1, Math.floor(scoreValue));
    if (safeScore <= 0) {
      return;
    }

    if (!(recordModal instanceof HTMLElement) || !recordModalInstance) {
      void promptAndSubmitFallback(safeScore);
      return;
    }

    pendingScoreToSubmit = safeScore;
    clearModalValidation();
    setRecordSaveBusy(false);

    if (recordModalText instanceof HTMLElement) {
      recordModalText.textContent = `${recordPromptText} ${recordScorePrefix}: ${safeScore}`;
    }

    if (recordNameInput instanceof HTMLInputElement) {
      recordNameInput.value = readPlayerName();
    }

    recordModalInstance.show();
  }

  let bestScore = readBest();
  updateHud(0, bestScore);

  let running = false;
  let paused = false;
  let gameOver = false;

  let score = 0;
  let speed = SPEED.base;

  let dinoY = GROUND_Y - DINO.h;
  let dinoVy = 0;
  let onGround = true;

  /** @type {Array<{x:number,y:number,w:number,h:number,arm:null|'left'|'right'}>} */
  let obstacles = [];
  let spawnTimer = 0;
  let nextSpawn = rand(SPAWN.min, SPAWN.max);

  let lastTs = 0;
  let stepAccum = 0;
  let stepPhase = false;

  function resetGame() {
    score = 0;
    speed = SPEED.base;
    dinoY = GROUND_Y - DINO.h;
    dinoVy = 0;
    onGround = true;
    obstacles = [];
    spawnTimer = 0;
    nextSpawn = rand(SPAWN.min, SPAWN.max);
    stepAccum = 0;
    stepPhase = false;
  }

  function startGame() {
    if (paused) {
      paused = false;
    }

    if (gameOver || !running) {
      if (gameOver) {
        resetGame();
      }
      gameOver = false;
      running = true;
      applyOverlayText('hidden');
    }
  }

  function endGame() {
    running = false;
    gameOver = true;

    const finalScore = Math.floor(score);
    const isNewPersonalBest = finalScore > bestScore;

    if (isNewPersonalBest) {
      bestScore = finalScore;
      writeBest(bestScore);
    }

    updateHud(finalScore, bestScore);
    applyOverlayText('gameover');

    if (isNewPersonalBest) {
      promptToSaveRecord(finalScore);
    }
  }

  function togglePause() {
    if (gameOver) return;

    paused = !paused;
    if (paused) {
      applyOverlayText('paused');
    } else {
      applyOverlayText('hidden');
    }
  }

  function jump() {
    if (!onGround) {
      return;
    }

    dinoVy = PHYSICS.jumpVelocity;
    onGround = false;
  }

  function spawnObstacle() {
    const w = rand(18, 34);
    const h = rand(34, 64);
    const armChance = Math.random();
    const arm = armChance < 0.35 ? (Math.random() < 0.5 ? 'left' : 'right') : null;

    obstacles.push({
      x: WORLD_WIDTH + 16,
      y: GROUND_Y - h,
      w,
      h,
      arm
    });
  }

  function update(dt) {
    score += dt * 10;
    speed = clamp(SPEED.base + score * SPEED.ramp, SPEED.base, SPEED.max);

    dinoVy += PHYSICS.gravity * dt;
    dinoY += dinoVy * dt;
    if (dinoY >= GROUND_Y - DINO.h) {
      dinoY = GROUND_Y - DINO.h;
      dinoVy = 0;
      onGround = true;
    }

    for (const ob of obstacles) {
      ob.x -= speed * dt;
    }
    obstacles = obstacles.filter((ob) => ob.x + ob.w > -20);

    spawnTimer += dt;
    if (spawnTimer >= nextSpawn) {
      spawnTimer = 0;
      const difficulty = clamp(1 + score / 220, 1, 2.3);
      nextSpawn = rand(SPAWN.min, SPAWN.max) / difficulty;
      spawnObstacle();
    }

    if (onGround) {
      stepAccum += dt * (speed / 220);
      if (stepAccum >= 0.25) {
        stepAccum = 0;
        stepPhase = !stepPhase;
      }
    }

    const dinoRect = {
      x: DINO.x + 2,
      y: dinoY + 2,
      w: DINO.w - 4,
      h: DINO.h - 2
    };

    for (const ob of obstacles) {
      const obRect = {
        x: ob.x + 2,
        y: ob.y + 2,
        w: ob.w - 4,
        h: ob.h - 2
      };

      if (rectsIntersect(dinoRect, obRect)) {
        endGame();
        break;
      }
    }

    updateHud(score, bestScore);
  }

  function draw() {
    ctx.clearRect(0, 0, WORLD_WIDTH, WORLD_HEIGHT);
    drawGround();

    for (const ob of obstacles) {
      drawObstacle(ob);
    }

    drawDino({ dinoY }, stepPhase);

    const colors = themeColors ?? (themeColors = readThemeColors());
    ctx.fillStyle = colors.text;
    ctx.font = '16px ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace';
    ctx.textAlign = 'right';
    ctx.fillText(String(Math.floor(score)), WORLD_WIDTH - 14, 24);
  }

  function loop(ts) {
    if (!lastTs) lastTs = ts;
    const rawDt = (ts - lastTs) / 1000;
    lastTs = ts;

    const dt = clamp(rawDt, 0, 0.05);

    if (running && !paused && !gameOver) {
      update(dt);
    }

    draw();
    requestAnimationFrame(loop);
  }

  function handleAction() {
    if (isModalOpen) {
      return;
    }

    if (gameOver || !running) {
      startGame();
      return;
    }

    if (!paused) {
      jump();
    }
  }

  function restart() {
    resetGame();
    gameOver = false;
    paused = false;
    running = false;
    applyOverlayText('start');
    updateHud(0, bestScore);
  }

  container.addEventListener('pointerdown', (e) => {
    if (e.button !== 0 || isModalOpen) return;
    handleAction();
  });

  window.addEventListener('keydown', (e) => {
    if (isModalOpen || isTypingTarget(e.target)) return;

    const key = e.key;
    if (key === ' ' || key === 'ArrowUp') {
      e.preventDefault();
      handleAction();
      return;
    }

    if (key === 'r' || key === 'R') {
      restart();
      return;
    }

    if (key === 'p' || key === 'P') {
      togglePause();
      return;
    }
  });

  if (restartBtn instanceof HTMLButtonElement) {
    restartBtn.addEventListener('click', () => restart());
  }

  if (
    recordModal instanceof HTMLElement &&
    window.bootstrap &&
    typeof window.bootstrap.Modal === 'function'
  ) {
    recordModalInstance = window.bootstrap.Modal.getOrCreateInstance(recordModal, {
      backdrop: 'static'
    });

    recordModal.addEventListener('shown.bs.modal', () => {
      isModalOpen = true;
      if (recordNameInput instanceof HTMLInputElement) {
        recordNameInput.focus();
        recordNameInput.select();
      }
    });

    recordModal.addEventListener('hidden.bs.modal', () => {
      isModalOpen = false;
      pendingScoreToSubmit = null;
      clearModalValidation();
      setRecordSaveBusy(false);
    });
  }

  if (recordForm instanceof HTMLFormElement) {
    recordForm.addEventListener('submit', (e) => {
      e.preventDefault();
      void submitScoreFromModal();
    });
  }

  if (recordSaveBtn instanceof HTMLButtonElement) {
    recordSaveBtn.addEventListener('click', () => {
      void submitScoreFromModal();
    });
  }

  if (recordNameInput instanceof HTMLInputElement) {
    recordNameInput.addEventListener('input', () => clearModalValidation());
  }

  window.addEventListener('resize', () => resizeCanvas());

  try {
    const observer = new MutationObserver(() => {
      themeColors = readThemeColors();
    });
    observer.observe(document.documentElement, { attributes: true, attributeFilter: ['data-theme'] });
  } catch {
    // ignore
  }

  try {
    window
      .matchMedia('(prefers-color-scheme: dark)')
      .addEventListener('change', () => (themeColors = readThemeColors()));
  } catch {
    // ignore
  }

  themeColors = readThemeColors();
  resizeCanvas();
  applyOverlayText('start');
  void loadLeaderboard();
  requestAnimationFrame(loop);
})();
