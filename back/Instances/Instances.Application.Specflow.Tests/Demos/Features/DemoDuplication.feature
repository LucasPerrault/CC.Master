Feature: Demo duplication

Scenario: Create a demo from virgin as a user with scope "AllDepartments"
    Given a user with department code 'LUCCA'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then user should not get an error
        And demo duplication should exist for distributor 'LUCCA'
        And instance duplication should exist for subdomain 'aperture-science'
        And dns propagation should start for subdomain 'aperture-science'
        And one duplication should be found as pending for subdomain 'aperture-science'

Scenario: Create a demo from virgin as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'DISTRIBUTOR'
        Then user should not get an error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'
        And dns propagation should start for subdomain 'aperture-science'
        And one duplication should be found as pending for subdomain 'aperture-science'

Scenario: Create a demo for forbidden distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then user should get an error containing 'Insufficient rights to duplicate demo for another department than your own'

Scenario: Create a demo from other distributor as a user with scope "AllDepartments"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I request creation of demo 'aperture-science' by duplicating demo 'demo-lucca' for distributor 'DISTRIBUTOR'
        Then user should not get an error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'
        And dns propagation should start for subdomain 'aperture-science'
        And one duplication should be found as pending for subdomain 'aperture-science'

Scenario: Create a demo from own distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'demo-distributor' for distributor 'DISTRIBUTOR'
        Then user should not get an error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'
        And dns propagation should start for subdomain 'aperture-science'
        And one duplication should be found as pending for subdomain 'aperture-science'

Scenario: Create a demo virgin as a non-lucca user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'DISTRIBUTOR'
        Then user should not get an error
        And demo duplication should exist for distributor 'DISTRIBUTOR'
        And instance duplication should exist for subdomain 'aperture-science'
        And dns propagation should start for subdomain 'aperture-science'
        And one duplication should be found as pending for subdomain 'aperture-science'

Scenario: Create a demo from forbidden distributor as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I request creation of demo 'aperture-science' by duplicating demo 'demo-lucca' for distributor 'DISTRIBUTOR'
        Then user should get an error containing 'Source demo 2 could not be found'
        
Scenario: Create a demo from virgin as a user with scope "AllDepartments" as a similar creation is running
    Given a running duplication for demo 'aperture-science' of id 'deadbeef-0053-41a7-b607-c545afc2dad9'
    And a user with department code 'LUCCA'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then one duplication should be found as pending for subdomain 'aperture-science'
        And user should get an error containing 'Subdomain aperture-science is not available'
        
Scenario: Create a demo from virgin as a user with scope "AllDepartments" as a similar demo exists
    Given an existing demo 'aperture-science'
    And a user with department code 'LUCCA'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I request creation of demo 'aperture-science' by duplicating demo 'virgin' for distributor 'LUCCA'
        Then user should get an error containing 'Subdomain aperture-science is not available'
