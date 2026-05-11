使用指定工具打开浏览器，访问目标网页 https://plus.eleadmin.com/login 
该页面已自动预填充用户名"admin"、密码"admin"及验证码信息，直接点击登录按钮以进入系统后台首页。

1. 全面探索系统页面架构：
   - 分析登录页面与后台主页的完整结构，包括但不限于左侧导航菜单、顶部功能导航栏及主内容展示区域
   - 详细记录侧边栏菜单的层级结构（一级/二级/三级菜单）、展开/收起交互方式、选中状态样式及切换逻辑
   - 解析顶部导航栏组成元素，包括用户信息展示区、通知中心、系统设置入口等模块的布局与交互机制
   - 研究内容区域的标准组成，包括页面标题规范、内容布局模式及数据展示方式

2. 深入分析左侧边栏菜单系统：
   - Dashboard模块：详细记录工作台、分析页、监控页三个子页面的内容区域组成、数据展示组件及交互逻辑
   - 系统管理模块：全面分析用户管理、角色管理、菜单管理、机构管理、字典管理、文件管理、登录日志、操作日志八个二级菜单的页面结构、子页面层级、数据项定义、表单交互逻辑及权限控制机制，重点反推其组织架构设计、角色权限体系的实现方式，形成完整的页面结构文档、交互流程图及数据结构推测
   - 列表页面模块：记录所有二、三级菜单的展示样式、数据加载方式、筛选排序功能及分页交互逻辑

3. 研究内容区域导航系统：
   - 分析顶部tab标签页的生成机制、切换逻辑、持久化策略及溢出处理方式
   - 记录标签页的显示样式、激活状态标识及关闭/固定等操作交互

4. 系统全面信息收集：
   - 采用递进式探索方法，收集所有可访问页面的结构信息、交互逻辑及数据展示模式
   - 记录各功能模块间的关联关系及数据流转路径
   - 分析系统的响应式设计实现方式及跨设备兼容性处理

5. UI视觉元素分析：
   - 使用website-to-design-md技能提取系统的配色方案，包括主色、辅助色、中性色及功能色值定义，根据技能中的要求生成design.md
   - 记录字体家族、字号体系、行高规范及文本样式定义
   - 分析图标库使用情况、图标样式及交互反馈效果
   - 提取组件设计规范，包括按钮、表单、卡片、弹窗等基础元素的样式定义

执行方案：
1. 初始阶段：使用Chrome DevTools mcp手动探索登录流程及核心页面结构，记录关键DOM节点、CSS类名及交互事件
2. 进阶阶段：基于手动探索结果，使用Playwright编写自动化脚本，批量采集所有页面的结构信息、交互逻辑及视觉样式
3. 评估优化：对初步分析结果进行效果评估，如存在信息缺失或不准确情况，调整为完全使用Playwright进行系统性分析，或才你有更好的办法也可以。

交付要求：
- 在后台首页，使用website-to-design-md技能提取https://plus.eleadmin.com/login into design.md and design-preview.html.
- 分别生成用户管理、角色管理、菜单管理、机构管理、字典管理、文件管理、登录日志、操作日志模块的PRD文档，里面要包括功能设计，界面UI布局设计，数据结构设计，逻辑功能设计文档。
- 所有交付结果存放于/src目录下，分析过程中产生的临时文件（代码、截图、数据文件等）按类别存放于/src/temp子目录，完成后自动清理临时文件
- 需要在单独目录生成，从登录页到后台首页，包括后台整个UI框架结构完全复刻，交付为html单文件格式，相关子页面需要链接到对应的html文件，最终的结果是我可以拿来直接作用后台UI结构的复用。其中左侧边栏菜单，一级和二级及以下菜单的界面操作方式，导航和tab的切换方式等都需要完整复刻。
- 报告应包含足够详细的技术细节，确保能够基于分析结果进行系统重构与实现


使用指定工具打开浏览器，访问目标网页 https://plus.eleadmin.com/login 
该页面已自动预填充用户名"admin"、密码"admin"及验证码信息，直接点击登录按钮以进入系统后台首页。


---



原版参考：使用指定工具打开浏览器，访问目标网页 https://plus.eleadmin.com/login 
该页面已自动预填充用户名"admin"、密码"admin"及验证码信息，直接点击登录按钮以进入系统后台首页。

当代需要修改/添加的页面路径：src\html

先查看原版功能，再进行修改

TODO 1: 实现全屏按钮的全屏/恢复功能
参考原版按钮：
Page: /user/message

1. <svg> <svg>
   selector: #app > div > div:nth-of-type(2) > div:nth-of-type(1) > div:nth-of-type(4) > div:nth-of-type(1) > i > svg
   html: <svg viewBox="0 0 48 48" fill="none" stroke="currentColor" stroke-width="4.5" stroke-linecap="round" stroke-linejoin="round" style="stroke-width: 4;" data-ai-id="el-28"><path d="M7 17V7H17" data-ai-id

TODO 2: 实现语言切换图标点击后的下拉列表显示功能，暂时只做下拉显示，不做选择后的语言切换。
参考原版按钮：
Page: /user/message

1. .ele-admin-tool <div>
   selector: #app > div > div:nth-of-type(2) > div:nth-of-type(1) > div:nth-of-type(4) > div:nth-of-type(2)
   html: <div class="ele-admin-tool" data-ai-id="el-180"><div class="el-dropdown ele-dropdown-trigger" style="line-height: inherit;" data-ai-id="el-181"><div id="el-id-6171-19" role="button" tabindex="0" class

TODO 3: 实现通知按钮的点击显示功能
参考原版按钮：
Page: /user/message

1. .ele-admin-tool <div>
   selector: #app > div > div:nth-of-type(2) > div:nth-of-type(1) > div:nth-of-type(4) > div:nth-of-type(3)
   text: "13"
   html: <div class="ele-admin-tool" data-ai-id="el-186"><div data-v-7ba3c13c="" class="el-tooltip__trigger" style="display: flex; align-items: center; height: 100%;" data-ai-id="el-187"><div data-v-7ba3c13c="



要与原版的菜单图标相同
Page: /extension/upload

1. .ele-menu-trigger <div>
   selector: #app > div > div:nth-of-type(1) > div:nth-of-type(2) > div:nth-of-type(2) > div:nth-of-type(1) > div > ul > li:nth-of-type(1) > div
   html: <div class="ele-menu-trigger" data-ai-id="el-4"><a href="/dashboard/workplace" class="ele-menu-link" data-ai-id="el-5"></a></div>

Page: /extension/upload

1. .ele-menu-trigger <div>
   selector: #app > div > div:nth-of-type(1) > div:nth-of-type(2) > div:nth-of-type(2) > div:nth-of-type(1) > div > ul > li:nth-of-type(2) > div:nth-of-type(2)
   html: <div class="ele-menu-trigger" data-ai-id="el-16"><a href="/dashboard/analysis" class="ele-menu-link" data-ai-id="el-17"></a></div>      

Page: /extension/upload

1. .ele-menu-trigger <div>
   selector: #app > div > div:nth-of-type(1) > div:nth-of-type(2) > div:nth-of-type(2) > div:nth-of-type(1) > div > ul > li:nth-of-type(3) > div
   html: <div class="ele-menu-trigger" data-ai-id="el-25"><a href="/dashboard/monitor" class="ele-menu-link" data-ai-id="el-26"></a></div>   

还有就是个人中心下面二级菜单的图标也要与原版一样   