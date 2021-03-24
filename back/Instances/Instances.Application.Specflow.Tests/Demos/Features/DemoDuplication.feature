Feature: Demo duplication

Scenario: Create a demo from virgin as a user with scope "AllDepartments"
    Given a user with department code 'DISTRIBUTOR' and operation 'Demo' and scope 'AllDepartments'
    When I create new demo 'aperture-science' by duplicating demo 'virgin' for distributor 'DISTRIBUTOR'
        Then demo 'aperture-science' should exist for distributor 'DISTRIBUTOR'
