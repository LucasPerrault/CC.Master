Feature: Demos access

Scenario: Get a list of demos as a user with scope "AllDepartments"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I get the list of demos
        Then it should contain 'template' demos
        And it should contain 'regular' demos from distributor 'LUCCA'
        And it should contain 'regular' demos from distributor 'DISTRIBUTOR'

Scenario: Get a list of template demos as a user with scope "AllDepartments"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I get the list of 'template' demos
        Then it should contain 'template' demos

Scenario: Get a list of demos as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I get the list of demos
        Then it should contain 'template' demos
        And it should contain 'regular' demos from distributor 'DISTRIBUTOR'
        And it should not contain any 'regular' demos from distributor other than 'DISTRIBUTOR'

Scenario: Get a list of demos filtered with subdomain "demo-lucca"
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I get the list of demos for subdomain 'demo-lucca'
        Then it should contain demos with subdomain 'demo-lucca'
        And it should not contain any demo other than 'demo-lucca'
