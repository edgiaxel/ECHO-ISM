# Visual, UI/UX, and Audio Design — ECHO-ISM

## 1. Overall art direction

**Style:** 2D digital drawing with clean silhouettes, simple geometric construction, restrained texture, and a slightly atmospheric digital-world presentation.

The game should feel hand-designed and expressive without requiring complex character art or 3D assets. Use basic shapes as the first production pass, then refine the drawing style after the gameplay is stable.

### Visual principles

- Prioritize readability over decoration.
- Use a small, consistent palette.
- Give interactable objects recognizable silhouettes.
- Keep puzzle states visible from the normal camera view.
- Use outlines, glow, and shape changes to communicate state.
- Avoid excessive bloom, noise, particles, and screen effects that obscure the puzzle.
- Use the same visual language for the world, UI, and feedback.
- The environment should suggest a miniature digital city, not a realistic computer interface.

## 2. Character design: Greg

Greg is a small blob with eyes. The first version should be possible to draw using a few shapes.

### Base silhouette
- Rounded blob body, slightly irregular rather than a perfect circle.
- Two simple eyes with readable direction.
- Optional small mouth only if it improves emotion or feedback.
- No detailed limbs required initially.
- Movement can be expressed through squash, stretch, wobble, and a small trailing mark.

### Variants
- **Live Greg:** opaque, clearly outlined, primary visual focus.
- **Echo Greg:** translucent, slightly desaturated or hue-shifted, with a faint trailing echo.
- **Interaction state:** eyes or outline respond to nearby usable objects.
- **Success state:** brief bounce or glow.
- **Failure/reset state:** brief digital flicker or dissolve, without disturbing the player's ability to retry.

Do not copy the branding, silhouette, or exact appearance of any existing AI mascot. Use the general idea of an approachable abstract character with eyes, but create a distinct design.

## 3. World and environment

Use simple modular shapes:
- Rectangular and rounded rooms.
- Doors represented by clear gaps, panels, or barriers.
- Switches with distinct inactive and active states.
- Terminals with a small screen or signal symbol.
- Connections represented by lines, traces, or animated pulses.
- Hazards represented as abstract instability, corrupted signal areas, or warning zones.
- Decorative geometry should never be mistaken for a usable object.

### State language
- Idle: stable outline and low visual intensity.
- Available: subtle highlight or interaction indicator.
- Active: clear shape or lighting change.
- Locked: visible barrier and lock/connection cue.
- Verified: stable signal symbol or connected line.
- Unverified: broken, noisy, or incomplete signal indicator.
- Failed: brief red/orange warning or disrupted outline.
- Complete: stable glow and a concise confirmation animation.

These are semantic state descriptions, not fixed color requirements. The final palette must maintain contrast and remain readable for color-vision differences.

## 4. Camera and composition

- Top-down 2D.
- Keep the whole puzzle visible where practical.
- Use a fixed camera for small rooms; use smooth camera movement only if a larger level genuinely requires it.
- Do not rotate the camera during the initial prototype.
- Keep the player and active echo visually distinguishable.
- Avoid camera shake for ordinary interactions. Reserve it for rare, non-disorienting events if used at all.

## 5. UI/UX layout

### Main gameplay HUD
- **Loop timer:** prominent and readable.
- **Loop/echo counter:** show current attempt and active echo count.
- **Objective:** one concise sentence or icon-supported instruction.
- **Interaction prompt:** appears near an interactable object.
- **Echo timeline:** optional early UI element showing recorded attempts as separate tracks.
- **Reset/commit hint:** show the relevant control and explain whether it records the current attempt.
- **Status feedback:** indicate when a mechanism activates, a timing action fails, or an objective completes.

### Menus
- Title screen: game name, Play, Settings, Quit.
- Pause: Resume, Restart Level, Settings, Return to Title.
- Level completion: success message and Continue/Replay.
- Failure: brief explanation when useful, Retry.
- Settings: volume, fullscreen/windowed mode, and keybind display if supported.

Do not build elaborate menus before the core gameplay is stable.

## 6. Controls

Initial keyboard defaults:
- Move: WASD or arrow keys.
- Interact/confirm: E or Space, with one consistent primary interaction mapping.
- Reset/commit loop: R, with clear confirmation or an undo policy if accidental resets are likely.
- Pause: Escape.

The exact mappings should be configurable or easy to change. UI prompts must display the actual configured action where possible. Avoid relying exclusively on color or sound to communicate a puzzle state.

## 7. Echo presentation

Echoes are a core mechanic and must be easy to read.

- Use a translucent body and clear outline.
- Optionally display a faint motion trail.
- Keep echoes visually distinct from decorative effects.
- When an echo interacts, show a small pulse or mechanism response.
- If several echoes overlap, preserve enough contrast to distinguish them.
- Show whether an echo is active, waiting, interacting, or finished through a simple icon, animation, or timeline marker.
- Do not make echoes so transparent that their positions become hard to read.

## 8. Effects and atmosphere

The game may use restrained digital atmosphere:
- Soft glow around active nodes.
- Brief signal pulses along verified connections.
- Subtle scanline or grain treatment used sparingly.
- Small particles during reset or echo creation.
- A brief glitch effect when the city system destabilizes.
- Gradual visual recovery as a puzzle is solved.

### Gloom and instability
The city can look less stable as misinformation and broken connections spread:
- Dimming or flickering connection lines.
- Incomplete signal paths.
- Slightly desaturated background details.
- Abstract cracks or missing geometric fragments.
- Warning pulses around an event that is close to triggering.

### Recovery
As the player restores the system:
- Lines become continuous.
- Nodes return to a stable idle animation.
- Background elements regain clarity.
- The environment emits a calm confirmation pulse.

Effects must not obscure player position, timing targets, interactable states, or instructions. Keep post-processing inexpensive and optional.

## 9. Audio direction

Audio should support timing, state recognition, and atmosphere without becoming noisy.

### Music
- Minimal electronic ambience.
- Gentle rhythmic layers that can become more complete as the city recovers.
- Avoid overly dramatic horror or combat music.
- Use a short loop suitable for repeated puzzle attempts.

### Sound effects
- Movement: optional soft blip or no sound to avoid repetition fatigue.
- Interaction: distinct, short confirmation sound.
- Switch: mechanical click or digital pulse.
- Verification: clear confirmation tone.
- Failed timing: short, non-punitive feedback.
- Echo creation: brief reverse or layered digital sound.
- Echo interaction: subtle copy/phase sound.
- Door: short opening/closing sound.
- Objective complete: small musical resolution.
- System instability: low-intensity warning tone that is not excessively alarm-like.

Use original or properly licensed audio. Keep audio cues supplementary; the game must remain understandable when muted.

## 10. Accessibility and readability

- Provide high contrast between Greg, echoes, obstacles, and interactive objects.
- Do not encode essential information with color alone.
- Use shape, outline, icon, or text as secondary indicators.
- Provide enough time for players to understand the introductory puzzles.
- Use forgiving timing windows before introducing stricter challenges.
- Keep UI text concise and legible.
- Avoid intense flashing, prolonged screen shake, and excessive visual noise.
- Allow audio to be muted.

## 11. Asset workflow

1. Begin with Godot primitives and simple drawn shapes.
2. Test gameplay and screen readability.
3. Create a small palette and consistent outline language.
4. Refine Greg's silhouette and eye expressions.
5. Replace placeholder switches, doors, and terminals with simple original drawings.
6. Add effects only after state communication is clear.
7. Add music and sound effects after core interactions are stable.

Do not generate a large asset library before the first level is playable.

## 12. Promotional media

Remotion and/or Hyperframes may be used to create:
- A short gameplay trailer.
- A progress video with screen recordings.
- Titles, captions, and transitions.
- A concise explanation of the echo mechanic.

These tools are not part of the Godot runtime. Capture genuine gameplay footage from the project and do not present mockups as implemented features.
