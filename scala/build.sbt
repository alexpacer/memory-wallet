name := "memory-wallet"

version := "0.1"

scalaVersion := "2.12.8"

libraryDependencies ++= Seq(
  "com.typesafe.akka" %% "akka-actor" % "2.5.19",
  "com.typesafe.akka" %% "akka-persistence" % "2.5.19",
  "com.typesafe.akka" %% "akka-testkit" % "2.5.19" % Test
)

libraryDependencies += "com.geteventstore" %% "akka-persistence-eventstore" % "5.0.4"
