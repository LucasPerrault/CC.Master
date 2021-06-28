Feature: Demo duplication end

    Scenario: Notify demo duplication has ended
        Given a running duplication for demo 'aperture-science' of id 'deadbeef-0053-41a7-b607-c545afc2dad9'
        When I get notification that duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' has ended
        Then duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' should result in instance creation
        And duplication 'deadbeef-0053-41a7-b607-c545afc2dad9'' should not result in instance deletion
        And duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' should be marked as FinishedWithSuccess

    Scenario: Cleanup when password reset fails
        Given a running duplication for demo 'aperture-science' of id 'deadbeef-0053-41a7-b607-c545afc2dad9'
        When duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' ends but password reset fails
        Then duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' should result in instance creation
        And duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' should result in instance deletion
        And duplication 'deadbeef-0053-41a7-b607-c545afc2dad9' should be marked as FinishedWithFailure
        And no demo 'aperture-science' should be active
