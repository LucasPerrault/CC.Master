Feature: Demos access

Scenario: Get a list of demos as a user with scope "AllDepartments"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I get the list of demos
        Then it should contain 'template' demos
        And it should contain 'regular' demos from 'LUCCA'
        And it should contain 'regular' demos from 'DISTRIBUTOR'

Scenario: Get a list of demos as a user with scope "DepartmentOnly"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly'
    When I get the list of demos
        Then it should contain 'template' demos
        And it should contain 'regular' demos from 'DISTRIBUTOR'
        And it should not contain any 'regular' demos from other distributors than 'DISTRIBUTOR'

