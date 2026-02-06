import { Redirect } from "expo-router";

/**
 * Tabs index: redirect to the workout (first) tab so /(tabs) resolves.
 */
export default function TabsIndex() {
  return <Redirect href="/(tabs)/workout" />;
}
