Feature: Demos deletion

Scenario: Delete regular demo
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDistributors'
    When I delete 'regular' demo from distributor 'LUCCA'
    And I get the list of demos
    Then demo 'demo-lucca' should not exist

Scenario: Delete regular demo in restricted scope
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'OwnDistributorOnly '
    When I delete 'regular' demo from distributor 'DISTRIBUTOR'
    And I get the list of demos
    Then demo 'demo-distributor' should not exist

Scenario: Delete regular demo in forbidden scope
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'OwnDistributorOnly '
    When I delete 'regular' demo from distributor 'LUCCA'
    Then user should get an error containing 'Exception of type 'Lucca.Core.Shared.Domain.Exceptions.NotFoundException' was thrown'

Scenario: Delete template demo
    Given a user with department code 'LUCCA' and operation 'Demo' and scope 'AllDistributors'
    When I delete 'template' demo from distributor 'LUCCA'
    Then user should get an error containing 'Template demos cannot be deleted'
