Feature: Demos deletion

Scenario: Delete regular demo
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I delete demo 'demo-lucca'
    And I get the list of demos
    Then demo 'demo-lucca' should not exist

Scenario: Delete regular demo in restricted scope
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly '
    When I delete demo 'demo-distributor'
    And I get the list of demos
    Then demo 'demo-distributor' should not exist

Scenario: Delete regular demo in forbidden scope
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'DepartmentOnly '
    When I delete demo 'demo-lucca'
    Then user should get error containing 'Exception of type 'Lucca.Core.Shared.Domain.Exceptions.NotFoundException' was thrown'

Scenario: Delete template demo
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I delete demo 'virgin'
    Then user should get error containing 'Template demos cannot be deleted'
