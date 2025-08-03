# JokesCollector

To execute:
1. Run scaffolding that should create a SQLite db
2. Add Key to user secrets
3. The run the function app

TODO:
* Add Azure Key Vault support for the API secret to be hidden after deployment

General comments:
* I like to solve cases with timer and retry logic but leveraging infrastructure - timer just sends messages to ASB, then we get retries and good alerting on DLQ for "free", in here for simplicity I did not consider this
* Batch size could be understood in different ways:
  * amount of calls to external api 
  * amount of jokes intended to be inserted into database
  
  The latter seemed more natural to me, this made the code a bit uglier as some parts of validation depend on the database
* I assumed when it comes to transactional approach it's on a joke level not batch level, thus the approach with failing when we reach max retries on either of the batch elements
* Inserting data one by one is a lot less efficient then inserting them at once, but after other assumptions I decided that readability of code trumps efficency unless stated otherwise (especially that this is a background process)
* Not really sure what kind of "error handler" should be implemented here as functions generally handle error reporting and logging pretty well
* Checking for duplicates could also be solved by reading a row before inserting or reading all before processor is run. I like unique index because that's a strong enforcement and there is always a chance that between select and insert something else will insert data
* Swapping the API can be done on the IoC level, just another API that implements the interface should be registered
* A lot of thiings in options have default values in code rather then local.settings.jon - I find that especially in larger apps those settings can grow a lot and most configurable settings never change. This pattern allows to always overrite the values but not have all of them as e.g. pipeline variables.
