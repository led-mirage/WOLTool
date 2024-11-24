# テストケース

## 正常系のテスト

- Test Case: MACアドレスのみを指定して実行
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55
    - 期待値: Magic Packetが正常に送信される。
    - 確認: マジックパケット送信後、ウェイトなしで終了すること。
    - 結果: OK
- Test Case: MACアドレスとホスト名を指定して実行
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55 --hostname 192.168.1.10
    - 期待値: Magic Packetが正常に送信される。
    - 確認: マジックパケット送信後、ホストの起動を待つこと。ホストが起動したら、所要時間が表示されること。
    - 結果: OK

## 引数の検証テスト

- Test Case: 必須引数（mac_address）が省略された場合
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55
    - 期待値: エラーメッセージが表示される。
    - 確認: "MAC address are required." のメッセージが表示されること。
    - 結果: OK
- Test Case: 無効なMACアドレスが渡された場合
    - 入力例: WOLTool --mac_address invalid_mac
    - 期待値: エラーメッセージが表示される。
    - 確認: "Invalid MAC address format." のメッセージが表示されること。
    - 結果: OK
- Test Case: 無効なブロードキャストアドレスが渡された場合
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55 --broadcast invalid_broadcast
    - 期待値: エラーメッセージが表示される。
    - 確認: "Invalid broadcast address format." のメッセージが表示されること。
    - 結果: OK
- Test Case: 無効なポート番号が渡された場合
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55 --port abc
    - 期待値: エラーメッセージが表示される。
    - 確認: "Invalid port number." のメッセージが表示されること。
    - 結果: OK
- Test Case: 無効なタイムアウト値が渡された場合
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55 -timeout abc
    - 期待値: エラーメッセージが表示される。
    - 確認: "Invalid timeout value." のメッセージが表示されること。
    - 結果: OK

## 異常系のテスト

- Test Case: マジックパケットの送信が失敗した場合
    - 入力: ネットワークエラーをシミュレート
    - 期待値: MagicPacketExceptionがスローされる。
    - 確認: "Failed to send magic packet." のメッセージが表示されること。
    - 結果: OK
- Test Case: ホストが既に起動している場合
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55 -hostname 192.168.1.10 
    - 期待値: ホストが起動しているメッセージが表示される。
    - 確認: "{hostname} is already up." のメッセージが表示されること。
    - 結果: OK

## オプションのテスト

- Test Case: --no-wait オプションの確認
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55 --hostname 192.168.1.10 --no_wait
    - 期待値: 起動待ちをせずに終了する。
    - 確認: "Magic packet sent." のメッセージが表示され、起動待ちをしないこと。
    - 結果: OK
- Test Case: --silent オプションの確認
    - 入力例: WOLTool --mac_address 00:11:22:33:44:55 --hostname 192.168.1.10 -silent
    - 期待値: 出力が抑制される。
    - 確認: コンソールに何も出力されないこと。
    - 結果: OK
- Test Case: --help オプションの確認
    - 入力例: WOLTool --help
    - 期待値: 使用方法のテキストが表示される。
    - 確認: usageが表示されること。
    - 結果: OK
