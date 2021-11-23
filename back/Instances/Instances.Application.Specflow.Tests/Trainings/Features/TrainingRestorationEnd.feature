@training
Feature: Training restoration end

Scenario Outline: Marking a restoration as completed when it's already completed raise an error
    Given a <progress> training restoration
    When I mark the restoration as completed <withOrWithout> success
    Then user should get an error containing ' was already marked as complete'

    Examples:
            | progress            | withOrWithout |
            | FinishedWithFailure | with          |
            | FinishedWithFailure | without       |
            | FinishedWithSuccess | with          |
            | FinishedWithSuccess | without       |

Scenario: Marking a restoration as completed with failure should log the result
    Given a Running training restoration
    When I mark the restoration as completed without success
    Then the restoration should be marked as FinishedWithFailure
    And a log should be created with TrainingRestorationFailed

Scenario: Marking a restoration as completed with success
    Given a Running training restoration
    When I mark the restoration as completed with success
    Then an instance of type training is created
    And a training object is created
    And a synchronization with WsAuth is launched
    And the cache of the training instance is reset
    And the restoration should be marked as FinishedWithSuccess
    And a log should be created with TrainingRestorationSucceeded

Scenario: Marking a restoration as completed with success but an error happens while finalizing
    Given a Running training restoration
    When I mark the restoration as completed but there is an error
    Then the restoration should be marked as FinishedWithFailure
    And a log should be created with TrainingRestorationFailed
