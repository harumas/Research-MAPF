# MAPF問題の研究リポジトリ

## 記事
[マルチエージェント経路計画の紹介](https://kei18.github.io/note/posts/mapf-tutorial/)

[マルチエージェント経路計画の基礎と最新動向](https://speakerdeck.com/ssii/ssii2023-os3-02)

## サンプルリポジトリ

[Multi-Agent-Path-Finding](https://github.com/GavinPHR/Multi-Agent-Path-Finding)  
[Multi-Agent path planning in Python](https://github.com/atb033/multi_agent_path_planning)

## 関連研究

### 日本語

[マルチエージェント経路探索のための厳密アルゴリズムの改良](https://www.jstage.jst.go.jp/article/pjsai/JSAI2022/0/JSAI2022_1N1GS503/_pdf/-char/ja)  

- 従来の経路探索アルゴリズム（MA-CBS）に対して、
新たなマージポリシーを提案する

[MAPDの解法TPにおける移動範囲の制限の緩和と集荷時間の推定に基づくタスク割り当ての改善](https://www.jstage.jst.go.jp/article/tjsai/37/3/37_37-3_A-L84/_pdf)

- インスタンス割当てに対する改良案

[マルチエージェント経路計画を柔軟に解くフレームワーク](https://ipsj.ixsq.nii.ac.jp/ej/?action=repository_uri&item_id=214826&file_id=1&file_no=1)

- PIBTを一般化し、様々なシナリオに対して柔軟に対応できる新しいフレームワーク

[MAPF問題におけるエージェントの操作回数を考慮する経路探索と退避先頂点の選択手法の検討](https://www.ieice.org/publications/conferences/summary.php?id=CONF0000129704&expandable=0&ConfCd=2021G&session_num=D-8&lecture_number=D-8-5&year=2021&conf_type=G)

- エージェントの操作回数を最小限に抑えるための手法の提案

[MAPD問題における滞在の予約と移動予測時間を用いるタスク割り当て](https://www.ieice.org/publications/conference-FIT-DVDs/FIT2020/data/pdf/F-033.pdf)

- 滞在の予約と移動予測時間を用いるタスク割り当て手法

[移動回数制限付きマルチエージェント経路発見問題の新しい定式化と解法](https://www.jstage.jst.go.jp/article/pjsai/JSAI2020/0/JSAI2020_2N4OS17a02/_pdf/-char/ja)

- エージェントの移動回数に制限を設けることで、限られたステップ内で目標地点に到達できるエージェントの数を最大化する手法に対する新しい定式化。

[時間拡張グラフ上のナンバーリンクパズルとしてのマルチエージェント経路発見](https://ipsj.ixsq.nii.ac.jp/ej/?action=repository_action_common_download&item_id=188687&item_no=1&attribute_id=1&file_no=1)

- MAPFを時間拡張グラフ上のナンバーリンクパズルと見なし，制約最適化問題に基づくMAPFの定式化を試みる．


[確率的に変動する所要時間下のマルチエージェント経路探索におけるパラメータ推定について](https://www.jstage.jst.go.jp/article/pjsai/JSAI2023/0/JSAI2023_2F4GS504/_article/-char/ja/)

[バトルロイヤルゲームのアイテム探索に適した マルチエージェント経路探索の定式化](https://gamescience.jp/2022/Paper/Shibayama_2022.pdf)

[MAPF問題におけるエージェントの操作回数を考慮する経路探索と退避先頂点の選択手法の検討](https://jglobal.jst.go.jp/detail?JGLOBAL_ID=202102220906834186)

[敵対者を考慮したマルチエージェント経路探索のためのアルゴリズム開発](https://kaken.nii.ac.jp/ja/grant/KAKENHI-PROJECT-17K12744/)

[連続時間のマルチエージェント経路探索](https://www.sciencedirect.com/science/article/pii/S0004370222000029?via%3Dihub)

### arXiv

[Explanation Generation for Multi-Modal Multi-Agent Path Finding with Optimal Resource Utilization using Answer Set Programming∗](https://arxiv.org/abs/2008.03573)

この論文は、自律倉庫などのロボティクス領域におけるmMAPF（Multi-Modal Multi-Agent Path Finding）問題に焦点を当てており、エージェントのバッテリーレベルや移動速度の変化を考慮する。ASP（Answer Set Programming）を使用して、エージェントの経路やリソース消費を最適化する方法を提案する。
- 道幅や、角度によって速度を変化させる。

[Variational collision and obstacle avoidance of multi-agent systems on Riemannian manifolds](https://arxiv.org/abs/1910.04995)

この論文の導入部分では、多機体システムの協力と調整が民間および軍事分野で広く応用されているため、その重要性が強調されている。特に、無人航空機や衛星クラスターの協力制御、群れの形成制御、センサーネットワークの制御などの応用が挙げられている。多数のロボットや自由度の多いシステムにおいて、最適な動作計画を見つけることは計算上非常に高コストになるため、簡単に計算可能な運動計画戦略の追求が重要である。そのため、幾何学的技術と変分解析に基づいた衝突回避戦略が提案されている。
- 衝突を起こさないような速度を算出する。

[Multi-agent Path Finding with Continuous Time Viewed Through Satisfiability Modulo Theories (SMT)](https://arxiv.org/abs/1903.09820)

Introduction and Backgroundセクションでは、マルチエージェント・パスファインディング（MAPF）の一般的な定義と、連続的な空間と時間におけるMAPFの新しい変種であるMAPF R（MAPF with continuous movements）に焦点が当てられています。MAPFは、エージェントが与えられた開始頂点から目標頂点まで移動するタスクであり、エージェント同士がグラフの頂点で衝突しないようにすることが目的です。MAPF Rは、最近導入されたMAPFの拡張であり、エージェントの移動が連続的であり、事前に定義された位置間での移動を考慮します。SMT推論をMAPF Rの解決に適用する方法を示し、Makespan最適解を目指しています。CBSアルゴリズムを基盤として、SMT-CBSアルゴリズムが導入され、効率的な解法が提案されています。

初期状態とゴール状態を元に、エージェントの移動可能な領域と速度を定義する。
それを元に、連続的な空間と時間におけるMAPF問題を解決する。
