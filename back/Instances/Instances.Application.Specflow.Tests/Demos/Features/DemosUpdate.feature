@demo
Feature: Demos update

Scenario: Update comment regular demo
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I update the comment to 'foo' on 'regular' demo from distributor 'LUCCA'
    Then demo comment should be 'foo'

Scenario: Update comment on demo in restricted scope
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I update the comment to 'foo' on 'regular' demo from distributor 'DISTRIBUTOR'
    Then demo comment should be 'foo'

Scenario: Update comment on demo in forbidden scope
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'Demo' with scope 'OwnDistributorOnly'
    When I update the comment to 'foo' on 'regular' demo from distributor 'LUCCA'
    Then user should get an error containing 'Exception of type 'Lucca.Core.Shared.Domain.Exceptions.NotFoundException' was thrown'

Scenario: Update comment on template demo
    Given a user with department code 'LUCCA'
    And user has operation 'Demo' with scope 'AllDistributors'
    When I update the comment to 'foo' on 'template' demo from distributor 'LUCCA'
    Then user should get an error containing 'Template demos cannot be updated'
