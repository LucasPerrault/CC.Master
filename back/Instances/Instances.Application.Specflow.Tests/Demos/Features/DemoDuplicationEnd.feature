Feature: Demo duplication end

Scenario: Notify demo duplication has ended
    Given a running duplication for demo 'aperture-science' of id 'deadbeef-0053-41a7-b607-c545afc2dad9'
    When I get notification that duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' has ended
        Then duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' should result in instance creation
