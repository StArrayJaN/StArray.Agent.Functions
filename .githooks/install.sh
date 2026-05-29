#!/usr/bin/env bash
# 将 .githooks/ 目录下的 hook 安装到 .git/hooks/
# Install hooks from .githooks/ to .git/hooks/
set -euo pipefail

HOOK_DIR="$(cd "$(dirname "$0")" && pwd)"
GIT_HOOK_DIR="$(git rev-parse --show-toplevel)/.git/hooks"

echo "Installing hooks from $HOOK_DIR to $GIT_HOOK_DIR"

for hook in "$HOOK_DIR"/*; do
    name=$(basename "$hook")
    # 跳过自身和本身是安装脚本的文件
    # Skip self and install scripts
    [[ "$name" =~ ^install ]] && continue
    [[ "$name" =~ \.(ps1|md)$ ]] && continue
    [[ ! -f "$hook" ]] && continue
    [[ ! -x "$hook" ]] && chmod +x "$hook"

    cp -f "$hook" "$GIT_HOOK_DIR/$name"
    chmod +x "$GIT_HOOK_DIR/$name"
    echo "  ✓ $name"
done

echo ""
echo "Hooks installed successfully!"
