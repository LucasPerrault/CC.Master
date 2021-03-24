Feature: Demo duplication

Scenario: Create a demo from virgin as a user with scope "AllDepartments"
    Given a user with department code 'LUCCA' and operation 'Demo' and scope 'AllDepartments'
    When I create new demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then demo 'aperture-science' should exist for distributor 'LUCCA'

Scenario: Create a demo from virgin as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I create new demo 'aperture-science' by duplicating demo 'virgin' for distributor 'DISTRIBUTOR'
        Then demo 'aperture-science' should exist for distributor 'DISTRIBUTOR'

Scenario: Create a demo for forbidden distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I create new demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then demo 'aperture-science' should not exist
        And user should get error containing 'Insufficient rights to duplicate demo for another department than your own'

Scenario: Create a demo from other distributor as a user with scope "AllDepartments"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I create new demo 'aperture-science' by duplicating demo 'demo-lucca' for distributor 'DISTRIBUTOR'
        Then demo 'aperture-science' should exist for distributor 'DISTRIBUTOR'

Scenario: Create a demo from own distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I create new demo 'aperture-science' by duplicating demo 'demo-distributor' for distributor 'DISTRIBUTOR'
        Then demo 'aperture-science' should exist for distributor 'DISTRIBUTOR'

Scenario: Create a demo virgin as a non-lucca user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I create new demo 'aperture-science' by duplicating demo 'virgin' for distributor 'DISTRIBUTOR'
        Then demo 'aperture-science' should exist for distributor 'DISTRIBUTOR'

Scenario: Create a demo from forbidden distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I create new demo 'aperture-science' by duplicating demo 'demo-lucca' for distributor 'DISTRIBUTOR'
        Then demo 'aperture-science' should not exist
        And user should get error containing 'Source demo demo-lucca could not be found'
