# Comandos recomendados para purgar secretos del historial Git

Precauciones antes de empezar:
- Hacer un respaldo completo del repositorio (`git clone --mirror`).
- Notificar al equipo: reescritura de historial obligará a todos a re-clonar.

1) Crear un mirror local (backup)

```bash
git clone --mirror <REPO_URL> repo-backup.git
```

2) Usar `git-filter-repo` (recomendado)

- Instalar: https://github.com/newren/git-filter-repo

```bash
# clonar espejo para modificar
git clone --mirror <REPO_URL> repo.git
cd repo.git

# preparar archivo de reemplazos (scripts/strings_to_replace.txt)
# ejecutar reemplazo (ejemplo reemplaza textos exactos)
git filter-repo --replace-text ../scripts/strings_to_replace.txt

# forzar push al remoto (ATENCIÓN: reescribe historial)
git push --force --all
git push --force --tags
```

3) Alternativa: usar BFG Repo-Cleaner

- Descargar BFG (https://rtyley.github.io/bfg-repo-cleaner/)

```bash
git clone --mirror <REPO_URL> repo.git
java -jar bfg.jar --replace-text ../scripts/strings_to_replace.txt repo.git
cd repo.git
git reflog expire --expire=now --all && git gc --prune=now --aggressive
git push --force --all
git push --force --tags
```

4) Pasos posteriores
- Pedir a todos los colaboradores que vuelvan a clonar el repositorio.
- Rotar cualquier credencial que haya estado expuesta (si no lo has hecho aún).

5) Verificación
- Ejecutar `git grep` en el nuevo clone para confirmar que no quedan apariciones.
