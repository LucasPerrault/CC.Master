Feature: Demos deletion

Scenario: Delete regular demo
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I delete demo 'demo-lucca'
    And I get the list of demos
    Then demo 'demo-lucca' should not exist

Scenario: Delete template demo
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I delete demo 'virgin'
    Then user should get error containing 'Template demos cannot be deleted'
