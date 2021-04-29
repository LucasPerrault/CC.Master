Feature: Demos deletion

Scenario: Delete regular demo
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I delete 'regular' demo from distributor 'LUCCA'
    And I get the list of demos
    Then demo 'demo-lucca' should not exist

Scenario: Delete regular demo in restricted scope
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly '
    When I delete 'regular' demo from distributor 'DISTRIBUTOR'
    And I get the list of demos
    Then demo 'demo-distributor' should not exist

Scenario: Delete regular demo in forbidden scope
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly '
    When I delete 'regular' demo from distributor 'LUCCA'
    Then user should get error containing 'Exception of type 'Lucca.Core.Shared.Domain.Exceptions.NotFoundException' was thrown'

Scenario: Delete template demo
    Given a user with department code 'LUCCA' and operation 'Demo' and scope 'AllDepartments'
    When I delete 'template' demo from distributor 'LUCCA'
    Then user should get error containing 'Template demos cannot be deleted'
