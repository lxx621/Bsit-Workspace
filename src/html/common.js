/* ==================== 公共交互函数 ==================== */

/* 树节点展开/折叠 */
function toggleTree(el) {
  const node = el.closest('.tree-node');
  node.classList.toggle('expanded');
  el.textContent = node.classList.contains('expanded') ? '▼' : '▶';
}

/* 打开弹窗 */
function openDialog(id) {
  document.getElementById(id).classList.add('show');
}

/* 关闭弹窗 */
function closeDialog(id) {
  document.getElementById(id).classList.remove('show');
}

/* 下拉菜单切换 */
function toggleDropdown(el) {
  const menu = el.closest('.dropdown-wrap').querySelector('.dropdown-menu');
  document.querySelectorAll('.dropdown-menu.show').forEach(m => {
    if (m !== menu) m.classList.remove('show');
  });
  menu.classList.toggle('show');
}

/* 点击外部关闭下拉菜单 */
document.addEventListener('click', function(e) {
  if (!e.target.closest('.dropdown-wrap')) {
    document.querySelectorAll('.dropdown-menu.show').forEach(m => m.classList.remove('show'));
  }
});

/* Switch开关切换 */
document.addEventListener('click', function(e) {
  const sw = e.target.closest('.switch');
  if (sw) {
    sw.classList.toggle('checked');
  }
});

/* 树节点选中 */
document.addEventListener('click', function(e) {
  const label = e.target.closest('.tree-label');
  if (label && label.closest('.tree-container')) {
    document.querySelectorAll('.tree-container .tree-node').forEach(n => n.classList.remove('active'));
    label.closest('.tree-node').classList.add('active');
  }
});

/* 树形表格展开/折叠 */
function toggleTreeTable(el, level) {
  const row = el.closest('tr');
  const isExpanded = el.textContent.trim() === '▼';
  el.textContent = isExpanded ? '▶' : '▼';
  
  let nextRow = row.nextElementSibling;
  while (nextRow) {
    const nextLevel = parseInt(nextRow.dataset.level || '0');
    if (nextLevel <= level) break;
    if (isExpanded) {
      nextRow.style.display = 'none';
    } else {
      nextRow.style.display = '';
    }
    nextRow = nextRow.nextElementSibling;
  }
}

/* 展开全部树形表格行 */
function expandAllTreeTable() {
  document.querySelectorAll('.data-table tbody tr[data-level]').forEach(tr => {
    tr.style.display = '';
  });
  document.querySelectorAll('.tree-table-toggle').forEach(t => {
    t.textContent = '▼';
  });
}

/* 折叠全部树形表格行 */
function collapseAllTreeTable() {
  document.querySelectorAll('.data-table tbody tr[data-level]').forEach(tr => {
    const level = parseInt(tr.dataset.level || '0');
    if (level > 0) tr.style.display = 'none';
  });
  document.querySelectorAll('.tree-table-toggle').forEach(t => {
    t.textContent = '▶';
  });
}
