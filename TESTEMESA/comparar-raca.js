const fs = require("fs");
const path = require("path");

const dir = __dirname;
const eolPath = path.join(dir, "eol.txt");   // blocos //turmaId + array JSON (fonte EOL)
const posPath = path.join(dir, "pos.txt");   // linhas (codigoAluno,RACA) (fonte atual/banco)
const outPath = path.join(dir, "comparacao-raca.csv");

// --- parse eol.txt ---
// varios blocos: linha "//<turmaId>" seguida de um array JSON ate a linha "]"
const eolRaw = fs.readFileSync(eolPath, "utf8");
const eol = new Map(); // codigoAluno -> { raca, codigoRaca, turmaId, nome }
{
  const lines = eolRaw.split(/\r?\n/);
  let turmaId = null;
  let buf = null;
  for (const line of lines) {
    const m = line.match(/^\/\/\s*(\d+)/);
    if (m) {
      turmaId = m[1];
      buf = null;
      continue;
    }
    if (line.trim() === "[") { buf = "["; continue; }
    if (buf !== null) {
      buf += "\n" + line;
      if (line.trim() === "]") {
        const arr = JSON.parse(buf);
        for (const a of arr) {
          eol.set(String(a.codigoAluno), {
            raca: (a.raca ?? "").trim().toUpperCase(),
            codigoRaca: a.codigoRaca ?? "",
            turmaId,
            nome: a.nomeAluno ?? "",
          });
        }
        buf = null;
      }
    }
  }
}

// --- parse pos.txt ---
const posRaw = fs.readFileSync(posPath, "utf8");
const pos = new Map(); // codigoAluno -> raca
for (const line of posRaw.split(/\r?\n/)) {
  const t = line.trim();
  if (!t) continue;
  const m = t.match(/^\((\d+)\s*,\s*(.+?)\)$/);
  if (!m) { console.warn("linha pos.txt ignorada:", line); continue; }
  pos.set(m[1], m[2].trim().toUpperCase());
}

// --- comparar ---
const allCodigos = new Set([...eol.keys(), ...pos.keys()]);
const rows = [];
let iguais = 0, diferentes = 0, soEol = 0, soPos = 0;

for (const cod of [...allCodigos].sort((a, b) => Number(a) - Number(b))) {
  const e = eol.get(cod);
  const pRaca = pos.get(cod);
  const eRaca = e ? e.raca : null;

  let status;
  if (eRaca == null) { status = "SO_POS"; soPos++; }
  else if (pRaca == null) { status = "SO_EOL"; soEol++; }
  else if (eRaca === pRaca) { status = "IGUAL"; iguais++; }
  else { status = "DIFERENTE"; diferentes++; }

  rows.push([
    cod,
    e ? e.turmaId : "",
    e ? e.nome : "",
    eRaca ?? "",
    e ? e.codigoRaca : "",
    pRaca ?? "",
    status,
  ]);
}

const header = ["codigoAluno", "turmaId", "nomeAluno", "racaEol", "codigoRacaEol", "racaPos", "status"];
const csv = [header, ...rows]
  .map(r => r.map(c => {
    const s = String(c);
    return /[",;\n]/.test(s) ? `"${s.replace(/"/g, '""')}"` : s;
  }).join(";"))
  .join("\n");

fs.writeFileSync(outPath, "﻿" + csv, "utf8");

console.log("gerado:", outPath);
console.log({ total: allCodigos.size, iguais, diferentes, soEol, soPos });
