@training
#@nullStringConversion
Feature: Training restoration

Scenario: Restore a training instance by a user with no "RestoreInstances" operation
    Given a user with no operation 'RestoreInstances'
    When I request the training restoration of environment 'no-existing-training'
        Then user should get an error containing 'Operation(s) RestoreInstances is(are) missing'

Scenario: Restore a training instance by a user with no "ReadEnvironments" operation for the environment
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'ReadEnvironments' with scope 'OwnDistributorOnly' for purpose 'Contractual'
    And user has operation 'RestoreInstances' with scope 'OwnDistributorOnly' for purpose 'Contractual'
    When I request the training restoration of environment 'lucca-only-environment'
        Then user should get a not found exception

Scenario: Restore a training instance by a user with no "RestoreInstances" operation for the environment
    Given a user with department code 'DISTRIBUTOR'
    And user has operation 'ReadEnvironments' with scope 'AllDistributors' for purpose 'Contractual'
    And user has operation 'RestoreInstances' with scope 'OwnDistributorOnly' for purpose 'Contractual'
    When I request the training restoration of environment 'lucca-only-environment'
        Then user should get an error containing 'Insufficient rights to restore the training instance of this environment'

Scenario: Restore a training instance when there was already an active training instance
    Given a user with department code 'LUCCA'
    And user has operation 'ReadEnvironments' with scope 'AllDistributors' for purpose 'Contractual'
    And user has operation 'RestoreInstances' with scope 'AllDistributors' for purpose 'Contractual'
    When I request the training restoration of environment 'with-previous-active-training'
        Then the previous training instance should be marked as deleted
        And the previous training should be marked as inactive
        And a backup of the previous training database should be made
        And a training restoration entry should be created
        And a duplication of the production instance of the environment should be started

Scenario: Restore a training instance when there is no active training instance
    Given a user with department code 'LUCCA'
    And user has operation 'ReadEnvironments' with scope 'AllDistributors' for purpose 'Contractual'
    And user has operation 'RestoreInstances' with scope 'AllDistributors' for purpose 'Contractual'
    When I request the training restoration of environment 'with-no-active-training'
        Then we should not try to delete an instance
        And a backup of the previous training database should be made
        And a training restoration entry should be created
        And a duplication of the production instance of the environment should be started

Scenario Outline: Restore a training instance with different options are correctly recorded
    Given a user with department code 'LUCCA'
    And user has operation 'ReadEnvironments' with scope 'AllDistributors' for purpose 'Contractual'
    And user has operation 'RestoreInstances' with scope 'AllDistributors' for purpose 'Contractual'
    When I request the training restoration of environment 'with-no-active-training' <withOrWithoutAnonymisation> anonymisation, <withOrWithoutKeepingExistingPasswords> keeping existing passwords, <withOrWithoutRestoringFiles> restoring files and the comment is '<comment>' with the expiry date <commentExpiryDate>
        Then the training restoration object stores the correct values

    Examples:
            | withOrWithoutAnonymisation | withOrWithoutKeepingExistingPasswords | withOrWithoutRestoringFiles            | comment           | commentExpiryDate |
            | without                    | without                               | without                                | this is a comment | 2021-11-28        |
            | without                    | without                               | with                                   | this is a comment | 2021-11-28        |
            | without                    | with                                  | without                                | this is a comment | 2021-11-28        |
            | without                    | with                                  | with                                   | this is a comment | 2021-11-28        |
            | with                       | without                               | without                                | this is a comment | <null>            |
            | with                       | without                               | with                                   | this is a comment | <null>            |
            | with                       | with                                  | without                                | this is a comment | <null>            |
            | with                       | with                                  | with                                   | this is a comment | <null>            |
            | with                       | with                                  | with                                   | <null>            | <null>            |

Scenario: Restore a training instance with no anonymisation should include only cleaning scripts
    Given a user with department code 'LUCCA'
    And user has operation 'ReadEnvironments' with scope 'AllDistributors' for purpose 'Contractual'
    And user has operation 'RestoreInstances' with scope 'AllDistributors' for purpose 'Contractual'
    When I request the training restoration of environment 'with-no-active-training' without anonymisation
    Then cleaning scripts should be applied
    And no anonymization scripts should be applied
    And no other scripts should be applied on the buffer server

Scenario: Restore a training instance with anonymisation should include cleaning scripts and anonymisation script
    Given a user with department code 'LUCCA'
    And user has operation 'ReadEnvironments' with scope 'AllDistributors' for purpose 'Contractual'
    And user has operation 'RestoreInstances' with scope 'AllDistributors' for purpose 'Contractual'
    When I request the training restoration of environment 'with-no-active-training' with anonymisation
    Then cleaning scripts should be applied
    And anonymization scripts should be applied
    And no other scripts should be applied on the buffer server
