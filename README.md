# ZFramework

········主要功能特点·······

资源加载：配置化自动打AB包，
编辑模式\包加载模式 资源存放\加载 无感切换

资源管理: 双层缓存池+资源组设计 利用引用计数 做到资源零死角追踪 彻底告别内存泄露，分组式卸载让特定资源的卸载时机更加可控

UI管理：采用树型结构，同父节点的节点间收发OnSwitch事件，纵向节点间收发OnCover、OnReveal事件（当栈用）,closeSelf方法递归调用，UI管理更加灵活

流程管理: 可为游戏定义多个大流程，使得游戏运行在一个可控的状态机下。

消息传递: 使用UniRx插件的Subject<T>实现
