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

  const WORLD_WIDTH = 900;
  const WORLD_HEIGHT = 240;
  const GROUND_Y = 190;

  const STORAGE_KEY_BEST = 'portfolio_dino_best';

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

    // Eye
    ctx.fillStyle = colors.accent;
    ctx.fillRect(x + 23, y + 10, 4, 4);

    // Legs (simple animation)
    ctx.fillStyle = colors.text;
    const legY = y + DINO.h - 6;
    const legA = stepPhase ? 2 : 0;
    ctx.fillRect(x + 8 + legA, legY, 6, 6);
    ctx.fillRect(x + 20 - legA, legY, 6, 6);
  }

  function drawObstacle(ob) {
    const colors = themeColors ?? (themeColors = readThemeColors());
    ctx.fillStyle = colors.text;

    // Body
    roundRect(ob.x, ob.y, ob.w, ob.h, 6);
    ctx.fill();

    // Arms (optional)
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

    if (score > bestScore) {
      bestScore = Math.floor(score);
      writeBest(bestScore);
    }

    updateHud(score, bestScore);
    applyOverlayText('gameover');
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

    // Dino physics
    dinoVy += PHYSICS.gravity * dt;
    dinoY += dinoVy * dt;
    if (dinoY >= GROUND_Y - DINO.h) {
      dinoY = GROUND_Y - DINO.h;
      dinoVy = 0;
      onGround = true;
    }

    // Obstacles
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

    // Simple step animation
    if (onGround) {
      stepAccum += dt * (speed / 220);
      if (stepAccum >= 0.25) {
        stepAccum = 0;
        stepPhase = !stepPhase;
      }
    }

    // Collision detection
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

    // Small score on canvas (top-right)
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

  // Events
  container.addEventListener('pointerdown', (e) => {
    if (e.button !== 0) return;
    handleAction();
  });

  window.addEventListener('keydown', (e) => {
    if (isTypingTarget(e.target)) return;

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

  window.addEventListener('resize', () => resizeCanvas());

  // Update theme colors when the toggle changes.
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

  // Init
  themeColors = readThemeColors();
  resizeCanvas();
  applyOverlayText('start');
  requestAnimationFrame(loop);
})();

