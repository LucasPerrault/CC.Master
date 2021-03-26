Feature: Demo duplication

Scenario: Create a demo from virgin as a user with scope "AllDepartments"
    Given a user with department code 'LUCCA' and operation 'Demo' and scope 'AllDepartments'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then user should not get error
        And demo duplication should exist for distributor 'LUCCA'
        And instance duplication should exist for subdomain 'aperture-science'

Scenario: Create a demo from virgin as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'DISTRIBUTOR'
        Then user should not get error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'

Scenario: Create a demo for forbidden distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then user should get error containing 'Insufficient rights to duplicate demo for another department than your own'

Scenario: Create a demo from other distributor as a user with scope "AllDepartments"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I request creation of demo 'aperture-science' by duplicating demo 'demo-lucca' for distributor 'DISTRIBUTOR'
        Then user should not get error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'

Scenario: Create a demo from own distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'demo-distributor' for distributor 'DISTRIBUTOR'
        Then user should not get error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'

Scenario: Create a demo virgin as a non-lucca user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'DISTRIBUTOR'
        Then user should not get error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'

Scenario: Create a demo from forbidden distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'demo-lucca' for distributor 'DISTRIBUTOR'
        Then user should get error containing 'Source demo demo-lucca could not be found'
