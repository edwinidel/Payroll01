#!/usr/bin/env bash

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
WATCH_MODE=0

if [[ "${1:-}" == "--watch" ]]; then
    WATCH_MODE=1
fi

PROJECT_NAMES=("2FA" "OvertimeCalculator")
PROJECT_PATHS=(
    "NominaH01/2FA/2FA.csproj"
    "OverTimeCalc01/OvertimeCalculator/OvertimeCalculator.csproj"
)
PROJECT_PORTS=(
    "5043 7094"
    "5031 7261"
)

PIDS=()
CLEANING_UP=0

kill_process_tree() {
    local pid=$1
    local children

    children=$(pgrep -P "$pid" 2>/dev/null || true)

    if [[ -n "$children" ]]; then
        for child in $children; do
            kill_process_tree "$child"
        done
    fi

    kill "$pid" 2>/dev/null || true
}

cleanup() {
    if [[ $CLEANING_UP -eq 1 ]]; then
        return
    fi

    CLEANING_UP=1
    local exit_code=${1:-0}

    if [[ ${#PIDS[@]} -gt 0 ]]; then
        echo
        echo "Deteniendo procesos..."

        for pid in "${PIDS[@]}"; do
            kill_process_tree "$pid"
        done

        sleep 1

        for pid in "${PIDS[@]}"; do
            if kill -0 "$pid" 2>/dev/null; then
                kill -9 "$pid" 2>/dev/null || true
            fi
        done

        wait "${PIDS[@]}" 2>/dev/null || true
    fi

    exit "$exit_code"
}

trap 'cleanup $?' EXIT
trap 'cleanup 130' INT TERM

ensure_port_is_free() {
    local project_name=$1
    local port=$2

    if lsof -n -iTCP:"$port" -sTCP:LISTEN >/dev/null 2>&1; then
        echo "Puerto ocupado para $project_name: $port"
        lsof -n -iTCP:"$port" -sTCP:LISTEN || true
        echo
        echo "Libera ese puerto antes de continuar o cierra la instancia previa del proyecto."
        exit 1
    fi
}

echo "Raíz: $ROOT_DIR"
echo "Modo: $([[ $WATCH_MODE -eq 1 ]] && echo watch || echo run)"
echo

for i in "${!PROJECT_PATHS[@]}"; do
    project_name="${PROJECT_NAMES[$i]}"
    project_ports="${PROJECT_PORTS[$i]}"

    for port in $project_ports; do
        ensure_port_is_free "$project_name" "$port"
    done
done

for i in "${!PROJECT_PATHS[@]}"; do
    project_name="${PROJECT_NAMES[$i]}"
    project_path="$ROOT_DIR/${PROJECT_PATHS[$i]}"

    echo "Iniciando $project_name"

    if [[ $WATCH_MODE -eq 1 ]]; then
        dotnet watch --project "$project_path" run &
    else
        dotnet run --project "$project_path" &
    fi

    PIDS+=("$!")
done

echo
echo "Ambos proyectos fueron lanzados. Presiona Ctrl+C para detenerlos."

set +e

first_exit_code=0

while true; do
    for pid in "${PIDS[@]}"; do
        if ! kill -0 "$pid" 2>/dev/null; then
            wait "$pid"
            first_exit_code=$?
            break 2
        fi
    done

    sleep 1
done

set -e

echo
echo "Uno de los proyectos finalizó con código $first_exit_code."
cleanup "$first_exit_code"