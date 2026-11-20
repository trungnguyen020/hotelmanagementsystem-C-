/* Engine vẽ sơ đồ lớp, sơ đồ hoạt động, sơ đồ tuần tự (SVG, trắng đen). */
(function () {
  const NS = "http://www.w3.org/2000/svg";
  const F = "Segoe UI, Arial, sans-serif";
  const CFG = window.DIAGRAM;

  function E(tag, attrs, parent) {
    const n = document.createElementNS(NS, tag);
    for (const k in attrs) {
      if (k === "text") n.textContent = attrs[k];
      else n.setAttribute(k, attrs[k]);
    }
    if (parent) parent.appendChild(n);
    return n;
  }

  const svg = E("svg", { width: CFG.width, height: CFG.height, viewBox: `0 0 ${CFG.width} ${CFG.height}`, xmlns: NS });
  document.body.appendChild(svg);
  E("rect", { x: 0, y: 0, width: CFG.width, height: CFG.height, fill: "#ffffff" }, svg);

  const defs = E("defs", {}, svg);
  function marker(id, open) {
    const m = E("marker", {
      id, viewBox: "0 0 12 12", refX: 11, refY: 6,
      markerWidth: 10, markerHeight: 10, orient: "auto-start-reverse"
    }, defs);
    E("path", open
      ? { d: "M1,1 L11,6 L1,11", fill: "none", stroke: "#111", "stroke-width": 1.4 }
      : { d: "M1,1 L11,6 L1,11 z", fill: "#111" }, m);
  }
  marker("arrow");
  marker("arrOpen", true);

  if (CFG.title) {
    E("text", { x: 28, y: 42, "font-family": F, "font-size": 22, "font-weight": 600, fill: "#111" }, svg)
      .textContent = CFG.title;
  }

  function wrap(s, max) {
    const words = String(s).split(" ");
    const lines = [];
    let line = "";
    for (const w of words) {
      if (!line.length) { line = w; continue; }
      if ((line + " " + w).length <= max) line += " " + w;
      else { lines.push(line); line = w; }
    }
    if (line.length) lines.push(line);
    return lines;
  }

  function textBlock(lines, x, y, opt) {
    opt = opt || {};
    const size = opt.size || 15;
    const lh = opt.lh || Math.round(size * 1.38);
    const t = E("text", {
      x, y, "font-family": F, "font-size": size, fill: opt.fill || "#111",
      "text-anchor": opt.anchor || "middle",
      "font-weight": opt.weight || 400
    }, opt.parent || svg);
    lines.forEach((ln, i) => {
      const s = E("tspan", { x, y: y + i * lh }, t);
      s.textContent = ln;
    });
    return t;
  }

  function labelBox(x, y, txt, opt) {
    opt = opt || {};
    const size = opt.size || 13;
    const w = txt.length * size * 0.56 + 8;
    const h = size + 6;
    let rx = x;
    if (opt.anchor === "middle") rx = x - w / 2;
    if (opt.anchor === "end") rx = x - w;
    E("rect", { x: rx, y: y - h + 4, width: w, height: h, fill: "#fff" }, svg);
    const t = E("text", {
      x: rx + 4, y: y - 2, "font-family": F, "font-size": size, fill: "#111",
      "font-weight": opt.weight || 400
    }, svg);
    t.textContent = txt;
  }

  function legend(items) {
    const y = CFG.height - 30;
    let x = 28;
    items.forEach(it => {
      if (it.kind === "line") {
        E("line", { x1: x, y1: y, x2: x + 42, y2: y, stroke: "#111", "stroke-width": 1.4 }, svg);
        const t = E("text", { x: x + 52, y: y + 5, "font-family": F, "font-size": 14, fill: "#111" }, svg);
        t.textContent = it.label;
        x += 52 + it.label.length * 7.6 + 34;
      } else if (it.kind === "dash") {
        E("line", { x1: x, y1: y, x2: x + 42, y2: y, stroke: "#111", "stroke-width": 1.4, "stroke-dasharray": "7 5" }, svg);
        E("path", { d: `M${x + 42},${y} l-9,-4 v8 z`, fill: "#111" }, svg);
        const t = E("text", { x: x + 52, y: y + 5, "font-family": F, "font-size": 14, fill: "#111" }, svg);
        t.textContent = it.label;
        x += 52 + it.label.length * 7.6 + 34;
      } else if (it.kind === "comp") {
        E("line", { x1: x, y1: y, x2: x + 42, y2: y, stroke: "#111", "stroke-width": 1.4 }, svg);
        E("polygon", { points: `${x},${y} ${x + 8},${y - 6} ${x + 16},${y} ${x + 8},${y + 6}`, fill: "#111", stroke: "#111" }, svg);
        const t = E("text", { x: x + 52, y: y + 5, "font-family": F, "font-size": 14, fill: "#111" }, svg);
        t.textContent = it.label;
        x += 52 + it.label.length * 7.6 + 34;
      }
    });
  }

  /* ---------------- SƠ ĐỒ LỚP ---------------- */
  function drawClass() {
    const boxes = {};
    (CFG.classes || []).forEach(c => {
      const w = c.w || 224;
      const attrs = c.attrs || [];
      const methods = c.methods || [];
      const x = c.x, y = c.y;
      let h = 38 + attrs.length * 21 + 10;
      if (methods.length) h += 10 + methods.length * 21;
      const b = { x, y, w, h, cx: x + w / 2, cy: y + h / 2 };
      boxes[c.id] = b;

      E("rect", { x, y, width: w, height: h, fill: "#fff", stroke: "#111", "stroke-width": 1.4 }, svg);
      E("line", { x1: x, y1: y + 34, x2: x + w, y2: y + 34, stroke: "#111", "stroke-width": 1.2 }, svg);
      const name = textBlock([c.name], x + w / 2, y + 24, { size: 16, weight: 600 });
      if (c.stereo) {
        name.setAttribute("y", y + 22);
        E("text", { x: x + w / 2, y: y + 22, "font-family": F, "font-size": 11.5, fill: "#333", "text-anchor": "middle" }, svg)
          .textContent = c.stereo;
      }
      attrs.forEach((a, i) => {
        E("text", { x: x + 12, y: y + 55 + i * 21, "font-family": F, "font-size": 14, fill: "#111" }, svg).textContent = a;
      });
      if (methods.length) {
        const my = y + 38 + attrs.length * 21 + 6;
        E("line", { x1: x, y1: my, x2: x + w, y2: my, stroke: "#111", "stroke-width": 1 }, svg);
        methods.forEach((m, i) => {
          E("text", { x: x + 12, y: my + 18 + i * 21, "font-family": F, "font-size": 13.5, fill: "#111" }, svg).textContent = m;
        });
      }
    });

    function border(b, tx, ty) {
      const dx = tx - b.cx, dy = ty - b.cy;
      if (!dx && !dy) return [b.cx, b.cy];
      const sx = dx ? (b.w / 2) / Math.abs(dx) : Infinity;
      const sy = dy ? (b.h / 2) / Math.abs(dy) : Infinity;
      const s = Math.min(sx, sy);
      return [b.cx + dx * s, b.cy + dy * s];
    }

    function sidePt(b, side) {
      if (side === "top") return [b.cx, b.y];
      if (side === "bottom") return [b.cx, b.y + b.h];
      if (side === "left") return [b.x, b.cy];
      return [b.x + b.w, b.cy];
    }

    (CFG.relations || []).forEach(r => {
      const a = boxes[r.from], b = boxes[r.to];
      if (!a || !b) return;
      let pts;
      if (r.via && r.via.length) {
        const start = border(a, r.via[0][0], r.via[0][1]);
        const last = r.via[r.via.length - 1];
        const end = border(b, last[0], last[1]);
        pts = [start].concat(r.via, [end]);
      } else {
        const start = r.fromSide ? sidePt(a, r.fromSide) : border(a, b.cx, b.cy);
        const end = r.toSide ? sidePt(b, r.toSide) : border(b, start[0], start[1]);
        pts = [start, end];
      }
      E("polyline", {
        points: pts.map(p => p.join(",")).join(" "),
        fill: "none", stroke: "#111", "stroke-width": 1.4
      }, svg);

      const p0 = pts[0], p1 = pts[1];
      const ux = (p1[0] - p0[0]), uy = (p1[1] - p0[1]);
      const len = Math.hypot(ux, uy) || 1;
      const nx = ux / len, ny = uy / len;
      if (r.type === "comp" || r.type === "agg") {
        const cx0 = p0[0] + nx * 16, cy0 = p0[1] + ny * 16;
        const px = -ny * 8, py = nx * 8;
        E("polygon", {
          points: `${p0[0]},${p0[1]} ${cx0 + px},${cy0 + py} ${cx0 + nx * 12},${cy0 + ny * 12} ${cx0 - px},${cy0 - py}`,
          fill: r.type === "comp" ? "#111" : "#fff", stroke: "#111", "stroke-width": 1.3
        }, svg);
      }
      if (r.type === "inherit") {
        const e0 = pts[pts.length - 1], e1 = pts[pts.length - 2];
        const vx = e0[0] - e1[0], vy = e0[1] - e1[1];
        const l2 = Math.hypot(vx, vy) || 1;
        const bx = e0[0] - (vx / l2) * 16, by = e0[1] - (vy / l2) * 16;
        const px = -vy / l2 * 8, py = vx / l2 * 8;
        E("polygon", { points: `${e0[0]},${e0[1]} ${bx + px},${by + py} ${bx - px},${by - py}`, fill: "#fff", stroke: "#111", "stroke-width": 1.3 }, svg);
      }
      if (r.multFrom) {
        labelBox(p0[0] + nx * 22 + ny * 16, p0[1] + ny * 22 - nx * 16, r.multFrom, { size: 13, anchor: "middle" });
      }
      if (r.multTo) {
        const e0 = pts[pts.length - 1], e1 = pts[pts.length - 2];
        const vx = e0[0] - e1[0], vy = e0[1] - e1[1];
        const l2 = Math.hypot(vx, vy) || 1;
        const tx = -vx / l2, ty = -vy / l2;
        labelBox(e0[0] + tx * 22 - ty * 16, e0[1] + ty * 22 + tx * 16, r.multTo, { size: 13, anchor: "middle" });
      }
    });

    if (CFG.note) {
      const n = CFG.note;
      const x = n.x || 28, y = n.y, w = n.w || 900;
      const lines = wrap(n.text, Math.floor(w / 7.4));
      const h = lines.length * 20 + 18;
      E("path", {
        d: `M${x},${y} h${w - 16} l16,16 v${h - 16} h-${w} z`,
        fill: "#fff", stroke: "#111", "stroke-width": 1.2
      }, svg);
      textBlock(lines, x + 10, y + 22, { size: 14, anchor: "start", parent: svg });
    }
    if (CFG.legend) legend(CFG.legend);
  }

  /* ---------------- SƠ ĐỒ HOẠT ĐỘNG ---------------- */
  function drawActivity() {
    const nodes = {};
    (CFG.nodes || []).forEach(n => {
      const kind = n.kind || "activity";
      if (kind === "activity") {
        const w = n.w || 340;
        const max = Math.floor((w - 28) / 7.3);
        const lines = wrap(n.text || "", max);
        const h = lines.length * 20 + 26;
        const x = n.x - w / 2, y = n.y - h / 2;
        E("rect", {
          x, y, width: w, height: h, rx: 14, ry: 14,
          fill: n.system ? "#f2f2f2" : "#ffffff",
          stroke: "#111", "stroke-width": 1.4,
          "stroke-dasharray": n.system ? "7 5" : "0"
        }, svg);
        textBlock(lines, n.x, n.y - (lines.length - 1) * 10 + 5, { size: 14.5 });
        nodes[n.id] = { kind, cx: n.x, cy: n.y, w, h };
      } else if (kind === "decision") {
        const hw = n.hw || 118, hh = n.hh || 52;
        const max = Math.floor((hw * 1.5) / 7.3);
        const lines = wrap(n.text || "", max);
        E("polygon", {
          points: `${n.x},${n.y - hh} ${n.x + hw},${n.y} ${n.x},${n.y + hh} ${n.x - hw},${n.y}`,
          fill: "#fff", stroke: "#111", "stroke-width": 1.4
        }, svg);
        textBlock(lines, n.x, n.y - (lines.length - 1) * 10 + 5, { size: 14 });
        nodes[n.id] = { kind, cx: n.x, cy: n.y, hw, hh };
      } else if (kind === "start") {
        E("circle", { cx: n.x, cy: n.y, r: 15, fill: "#111" }, svg);
        nodes[n.id] = { kind, cx: n.x, cy: n.y, r: 15 };
      } else if (kind === "end") {
        E("circle", { cx: n.x, cy: n.y, r: 20, fill: "#fff", stroke: "#111", "stroke-width": 2 }, svg);
        E("circle", { cx: n.x, cy: n.y, r: 11, fill: "#111" }, svg);
        nodes[n.id] = { kind, cx: n.x, cy: n.y, r: 20 };
      } else if (kind === "merge") {
        E("circle", { cx: n.x, cy: n.y, r: 10, fill: "#fff", stroke: "#111", "stroke-width": 1.4 }, svg);
        nodes[n.id] = { kind, cx: n.x, cy: n.y, r: 10 };
      } else if (kind === "fork") {
        E("rect", { x: n.x - 150, y: n.y - 4, width: 300, height: 8, fill: "#111" }, svg);
        nodes[n.id] = { kind, cx: n.x, cy: n.y, w: 300, h: 8 };
      }
    });

    function border(n, tx, ty) {
      const dx = tx - n.cx, dy = ty - n.cy;
      if (!dx && !dy) return [n.cx, n.cy];
      if (n.kind === "activity") {
        const sx = dx ? (n.w / 2) / Math.abs(dx) : Infinity;
        const sy = dy ? (n.h / 2) / Math.abs(dy) : Infinity;
        const s = Math.min(sx, sy);
        return [n.cx + dx * s, n.cy + dy * s];
      }
      if (n.kind === "decision") {
        const s = 1 / (Math.abs(dx) / n.hw + Math.abs(dy) / n.hh);
        return [n.cx + dx * s, n.cy + dy * s];
      }
      const r = n.r || 12;
      const l = Math.hypot(dx, dy);
      return [n.cx + dx / l * r, n.cy + dy / l * r];
    }

    function sidePoint(n, side) {
      if (n.kind === "activity") {
        if (side === "top") return [n.cx, n.cy - n.h / 2];
        if (side === "bottom") return [n.cx, n.cy + n.h / 2];
        if (side === "left") return [n.cx - n.w / 2, n.cy];
        return [n.cx + n.w / 2, n.cy];
      }
      if (n.kind === "decision") {
        if (side === "top") return [n.cx, n.cy - n.hh];
        if (side === "bottom") return [n.cx, n.cy + n.hh];
        if (side === "left") return [n.cx - n.hw, n.cy];
        return [n.cx + n.hw, n.cy];
      }
      return [n.cx, n.cy];
    }

    function polyMid(pts) {
      const segs = [];
      let total = 0;
      for (let i = 0; i < pts.length - 1; i++) {
        const l = Math.hypot(pts[i + 1][0] - pts[i][0], pts[i + 1][1] - pts[i][1]);
        segs.push({ a: pts[i], b: pts[i + 1], l });
        total += l;
      }
      let acc = 0;
      for (const s of segs) {
        if (acc + s.l >= total / 2) {
          const t = s.l ? (total / 2 - acc) / s.l : 0;
          return {
            x: s.a[0] + (s.b[0] - s.a[0]) * t,
            y: s.a[1] + (s.b[1] - s.a[1]) * t,
            vertical: Math.abs(s.b[0] - s.a[0]) < 1
          };
        }
        acc += s.l;
      }
      return { x: pts[0][0], y: pts[0][1], vertical: false };
    }

    (CFG.edges || []).forEach(e => {
      const a = nodes[e.from], b = nodes[e.to];
      if (!a || !b) return;
      const via = e.via || [];
      let start, end;
      if (e.fromSide) start = sidePoint(a, e.fromSide);
      else start = border(a, via.length ? via[0][0] : b.cx, via.length ? via[0][1] : b.cy);
      if (e.toSide) end = sidePoint(b, e.toSide);
      else {
        const last = via.length ? via[via.length - 1] : start;
        end = border(b, last[0], last[1]);
      }
      const pts = [start].concat(via, [end]);
      E("polyline", {
        points: pts.map(p => p.join(",")).join(" "),
        fill: "none", stroke: "#111", "stroke-width": 1.4,
        "marker-end": "url(#arrow)"
      }, svg);
      if (e.label) {
        const mid = polyMid(pts);
        if (mid.vertical) labelBox(mid.x + 8, mid.y + 5, e.label, { size: 13 });
        else labelBox(mid.x, mid.y - 8, e.label, { size: 13, anchor: "middle" });
      }
    });

    if (CFG.note) {
      const n = CFG.note;
      const x = n.x, y = n.y, w = n.w || 760;
      const lines = wrap(n.text, Math.floor(w / 7.4));
      const h = lines.length * 20 + 18;
      E("path", { d: `M${x},${y} h${w - 16} l16,16 v${h - 16} h-${w} z`, fill: "#fff", stroke: "#111", "stroke-width": 1.2 }, svg);
      textBlock(lines, x + 10, y + 22, { size: 14, anchor: "start" });
    }
  }

  /* ---------------- SƠ ĐỒ TUẦN TỰ ---------------- */
  function drawSequence() {
    const ps = CFG.participants || [];
    const top = 96;
    const step = CFG.step || ((CFG.width - 260) / (ps.length - 1 || 1));
    ps.forEach((p, i) => { p._x = p.x || 150 + i * step; });

    ps.forEach(p => {
      const x = p._x;
      if (p.kind === "actor") {
        E("circle", { cx: x, cy: top, r: 13, fill: "#fff", stroke: "#111", "stroke-width": 1.5 }, svg);
        E("line", { x1: x, y1: top + 13, x2: x, y2: top + 45, stroke: "#111", "stroke-width": 1.5 }, svg);
        E("line", { x1: x - 18, y1: top + 24, x2: x + 18, y2: top + 24, stroke: "#111", "stroke-width": 1.5 }, svg);
        E("line", { x1: x - 14, y1: top + 62, x2: x, y2: top + 45, stroke: "#111", "stroke-width": 1.5 }, svg);
        E("line", { x1: x + 14, y1: top + 62, x2: x, y2: top + 45, stroke: "#111", "stroke-width": 1.5 }, svg);
        textBlock(wrap(p.name, 16), x, top + 88, { size: 15, weight: 600 });
      } else {
        const w = p.w || 158;
        E("rect", { x: x - w / 2, y: top - 6, width: w, height: 42, fill: "#fff", stroke: "#111", "stroke-width": 1.4 }, svg);
        const lines = wrap(p.name, 20);
        textBlock(lines, x, top + (lines.length > 1 ? 8 : 20), { size: 14.5, weight: 600 });
      }
      const y0 = p.kind === "actor" ? top + 104 : top + 38;
      E("line", {
        x1: x, y1: y0, x2: x, y2: CFG.height - 46,
        stroke: "#111", "stroke-width": 1.1, "stroke-dasharray": "6 6"
      }, svg);
    });

    const y0 = CFG.messagesStart || 218;
    const gap = CFG.messageGap || 46;
    (CFG.messages || []).forEach((m, i) => {
      const y = y0 + i * gap;
      const a = ps.find(p => p.id === m.from);
      const b = ps.find(p => p.id === m.to);
      if (!a || !b) return;
      if (m.from === m.to) {
        const x = a._x;
        E("polyline", {
          points: `${x + 6},${y} ${x + 52},${y} ${x + 52},${y + 24} ${x + 6},${y + 24}`,
          fill: "none", stroke: "#111", "stroke-width": 1.3,
          "marker-end": "url(#arrow)"
        }, svg);
        labelBox(x + 60, y + 6, m.label || "", { size: 13 });
      } else {
        const x1 = a._x, x2 = b._x;
        const dashed = m.type === "return";
        const xa = x1 + (x2 > x1 ? 0 : -6);
        const xb = x2 + (x2 > x1 ? -6 : 6);
        E("line", {
          x1: xa, y1: y, x2: xb, y2: y,
          stroke: "#111", "stroke-width": 1.3,
          "stroke-dasharray": dashed ? "7 5" : "0",
          "marker-end": dashed ? "url(#arrOpen)" : "url(#arrow)"
        }, svg);
        if (m.label) {
          const mid = (x1 + x2) / 2;
          labelBox(mid, y - 8, m.label, { size: 13, anchor: "middle" });
        }
      }
    });

    if (CFG.note) {
      const n = CFG.note;
      const lines = wrap(n.text, Math.floor((n.w || 700) / 7.4));
      const h = lines.length * 20 + 18;
      const x = n.x, y = CFG.height - 40 - h - (CFG.legend ? 24 : 0);
      E("path", { d: `M${x},${y} h${(n.w || 700) - 16} l16,16 v${h - 16} h-${n.w || 700} z`, fill: "#fff", stroke: "#111", "stroke-width": 1.2 }, svg);
      textBlock(lines, x + 10, y + 22, { size: 14, anchor: "start" });
    }
    if (CFG.legend) legend(CFG.legend);
  }

  if (CFG.type === "class") drawClass();
  else if (CFG.type === "activity") drawActivity();
  else if (CFG.type === "sequence") drawSequence();
})();
