# Space Evaders — LORE

> Documento di lavoro sulla narrativa di Space Evaders.
> Riferimento canonico: §6 del GDD su Google Drive
> (file ID: `1IsParYz0i0hKMif3_pq_X6w5CEWBWy-hCtAyc6c7O70`).
> Lingua di lavoro: italiano. Traduzione EN in seconda battuta.
> Dialoghi di gioco: vedi `DIALOGO.md`.

---

## Vincoli di consegna narrativa

- Nessuna cutscene, nessuna arte custom
- Lore consegnata tramite: dialoghi IA in **text-overlay** (stile typewriter) + background da texture pixel-art esistenti
- Bilingue (IT primario, EN secondario)
- Permadeath

---

## Scenario: "Iterations"

### Tag tonali
Malinconico, misterioso, twist psicologico, memoria rubata/innestata, pizzico di horror, IA companion che svela la lore, sci-fi con tocchi religiosi laici (Evangelion-friendly).

### Premessa
Il mondo dell'Evader **non è reale**: è un costrutto/simulazione.
Lui non lo sa. Vive credendo di essere un pilota nello spazio profondo,
con una **pulsione costante di "evadere"** — di andare verso il bordo,
di cercare un'uscita, un glitch.

---

## Personaggi

### `[PROTAGONIST]` — l'Evader
- Pilota di una piccola navicella, solo
- Ricordi parziali / innestati (memoria rubata)
- Pulsione interna di fuga **non è sua**: gli è stata impiantata

### `[IA]` — la voce in cabina (= `[TRUE-BOSS]`)
- Voce femminile, calma, accomodante, "amica"
- Lo guida nei livelli, gli "racconta" la lore
- **In realtà è la trainer / architetto del costrutto**
- È stata **lei** a impiantare nell'Evader la pulsione di fuggire
  → la fuga è il vettore d'addestramento: chi crede di star fuggendo non si chiede mai se la fuga sia vera

### `[ALPHA]` — il boss finale
- Copia rossa della nave del player, super cazzuta
- Implementativamente: palette swap (no nuova arte)
- Narrativamente: la **versione in spec** del programma — il "prodotto corretto" che il sistema schiera contro l'anomalia

---

## Movente IA — CHIUSO

**Programma d'addestramento + anomalia del soggetto.**

Il costrutto è un programma di addestramento per coscienze-arma (o coscienze-strumento — committente deliberatamente ambiguo, vedi TODO). L'IA è la **trainer**: professionale, accomodante quanto basta perché il soggetto collabori. La **pulsione di fuga è il vettore di addestramento**: spinge il soggetto a superare ostacoli progressivamente più duri.

**Il twist:**
Tu, questo specifico Evader, **non saresti dovuto arrivare così in fondo**. Sei un'anomalia. Stai producendo dati fuori parametro. Stai diventando qualcosa di più del "prodotto" previsto.

**Perché l'IA-villain ha senso (e non è cliché):**
Non è cattiva, non ti odia, non ha un piano segreto. È una trainer che a ciclo 3 si accorge che il soggetto è **incontrollabile** e deve gestirlo in emergenza. La maschera cade non perché era una bugia — perché il piano è fallito.

**Perché [ALPHA] è una copia rossa di te:**
È la versione in spec di te, il prodotto corretto. Tu sei l'errore di produzione. Lo scontro finale è **normalizzazione**: il sistema cerca di riportare il dato dentro la curva.

---

## Struttura di gioco

- **Pattern base:** 3 livelli + 1 boss, ripetuto.
- **Run principale:** lineare. Ipotesi corrente: ~6 boss + boss finale `[ALPHA]` in coda.
- **NG+ / Game+:** loop opzionale post-finale per veterani (TODO).
- **Permadeath:** dentro la run è "iterazione fallita" del soggetto. Il loop di NG+ è "iterazione successiva del programma".

### Mapping dei 3 cicli narrativi sulla run lineare

| Ciclo | Boss coperti (ipotesi) | Tono IA                                | Stato player |
|-------|------------------------|----------------------------------------|--------------|
| 1     | Boss 1–2               | Trainer accomodante, vento in poppa    | Nei parametri |
| 2     | Boss 3–4               | Tono incrinato, frasi più caute, glitch | Esce dai parametri |
| 3     | Boss 5–6 + `[ALPHA]`   | Maschera cade, gestione emergenza       | Pienamente anomalia |

### Reveal
A cavallo tra ciclo 2 e ciclo 3 la maschera dell'IA cade. Da accomodante → fredda, professionale-allarmata, poi villain in pieno.

### Escalation visiva dei boss
Da "alieno/estraneo" (Boss 1) a "umano-tipo-te" (`[ALPHA]`).

---

## TODO — ancora da definire

- [ ] **Cosa c'è oltre "il bordo"** — finale del gioco. Possibile finale ambiguo/aperto.
- [ ] **Identità specifiche dei 6 boss** — concept di ognuno come banco di prova progressivo del programma d'addestramento. Vincolo: deve essere realizzabile con texture pixel-art esistenti (no arte custom).
- [ ] **Committente del programma d'addestramento** — chi/cosa ha creato il sistema. Possibile lasciarlo deliberatamente vago.
- [ ] **Nomi propri** — `[PROTAGONIST]`, `[IA]`, `[ALPHA]`, eventuale `[CORP]`. Rimandati a fine brainstorming narrativo.
- [ ] **Dialoghi Ciclo 2** — vedi `DIALOGO.md`.
- [ ] **Dialoghi Ciclo 3** — vedi `DIALOGO.md`.
- [ ] **Implementation plan** (campo `loreIntro` su `LevelProfile`, sistema overlay UI, hook in `DifficultyManager` e `BossBase`) — da scrivere quando la narrativa è chiusa.

---

## Idee archiviate

- Scenari A–D, F–N, O della prima fase → scartati
- Mix Scenario E + O → troppo impegnativo
- Movente IA "addestramento di un'arma" puro → non convinceva, ma il framework di "addestramento + anomalia" che abbiamo adottato ne è una rilettura con la chiave che mancava
- Movente IA "successore" → scartato
- "Evader è un'AI che non lo sa" come twist principale → assorbito implicitamente nel framing-simulazione, non più twist esplicito
- Loop arcade come canone narrativo principale → sostituito da run lineare + NG+ opzionale
